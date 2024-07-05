namespace d2;
unsafe

//======================================================================
// 几何部分
//======================================================================

// 点的定义
public struct ipixel_point_fixed
{
	cfixed x;
	cfixed y;
}

// 直线定义
public struct ipixel_line_fixed
{
	ipixel_point_fixed_t p1;
	ipixel_point_fixed_t p2;
}

// 矢量定义
unsafe public struct ipixel_vector
{
	public	fixed cfixed vector[3];
}

// 矩阵定义
unsafe public struct ipixel_transform
{
    public fixed cfixed matrix[3][3];
}

// 浮点数点
public struct ipixel_point
{
	double x;
	double y;
}

// 浮点矩阵
public struct ipixel_matrix
{
    public fixed double m[3][3];
}
 

// 梯形定义
public struct ipixel_trapezoid
{
	cfixed top, bottom;
	ipixel_line_fixed_t left, right;
}

// 扫描线定义
public struct ipixel_span
{
	int x, y, w;
}


public struct ipixel_edge
{
    cfixed x;
    cfixed e;
    cfixed stepx;
    cfixed signdx;
    cfixed dy;
    cfixed dx;
    cfixed stepx_small;
    cfixed stepx_big;
    cfixed dx_small;
    cfixed dx_big;
}


//---------------------------------------------------------------------
// 光栅化基础
//---------------------------------------------------------------------
#define ipixel_trapezoid_valid(t) \
((t).left.p1.y != (t).left.p2.y && \
		 (t).right.p1.y != (t).right.p2.y && \
		 (int)((t).bottom - (t).top) > 0)

	public const int
IPIXEL_SUBPIXEL_8	=	0,
IPIXEL_SUBPIXEL_4	=	1,
IPIXEL_SUBPIXEL_1	=	2;


//---------------------------------------------------------------------
// 几何基础
//---------------------------------------------------------------------

// 线段与y轴相交的x坐标，ceil为是否向上去整
public static inline cfixed ipixel_line_fixed_x(const ipixel_line_fixed_t *l,
						cfixed y, int ceil)
{
	cfixed dx = l.p2.x - l.p1.x;
	long ex = ((long)(y - l.p1.y)) * dx;
	cfixed dy = l.p2.y - l.p1.y;
	if (ceil) ex += (dy - 1);
	return l.p1.x + (cfixed)(ex / dy);
}

// 梯形在当前扫描线的X轴的覆盖区域
public static inline int ipixel_trapezoid_span_bound(const ipixel_trapezoid_t *t, 
		int y, int *lx, int *rx)
{
	cfixed x1, x2, y1, y2;
	int yt, yb;
	if (!ipixel_trapezoid_valid(t)) return -1;
	yt = cfixed_to_int(t.top);
	yb = cfixed_to_int(cfixed_ceil(t.bottom));
	if (y < yt || y >= yb) return -2;
	y1 = cfixed_from_int(y);
	y2 = cfixed_from_int(y) + cfixed_const_1_m_e;
	x1 = ipixel_line_fixed_x(&t.left, y1, 0);
	x2 = ipixel_line_fixed_x(&t.left, y2, 0);
	*lx = cfixed_to_int((x1 < x2)? x1 : x2);
	x1 = cfixed_ceil(ipixel_line_fixed_x(&t.right, y1, 1));
	x2 = cfixed_ceil(ipixel_line_fixed_x(&t.right, y2, 1));
	*rx = cfixed_to_int((x1 > x2)? x1 : x2);
	return 0;
}


// 很多梯形在当前扫描线的X轴的覆盖区域
public static inline int ipixel_trapezoid_line_bound(const ipixel_trapezoid_t *t,
	int n, int y, int *lx, int *rx)
{
	int xmin = 0x7fff, xmax = -0x7fff;
	int xl, xr, retval = -1;
	for (; n > 0; t++, n--) {
		if (ipixel_trapezoid_span_bound(t, y, &xl, &xr) == 0) {
			if (xl < xmin) xmin = xl;
			if (xr > xmax) xmax = xr;
			retval = 0;
		}
	}
	if (retval == 0) *lx = xmin, *rx = xmax;
	return retval;
}



//---------------------------------------------------------------------
// 位图低层次光栅化：透视/仿射变换
//---------------------------------------------------------------------
public const int
 IBITMAP_RASTER_FLAG_PERSPECTIVE	=	0,	// 透视变换
 IBITMAP_RASTER_FLAG_AFFINE		=	1,	// 反射变换

 IBITMAP_RASTER_FLAG_OVER		=	0,	// 绘制OVER
 IBITMAP_RASTER_FLAG_ADD			=	4,	// 绘制ADD
 IBITMAP_RASTER_FLAG_COPY		=	8;	// 绘制COPY



//=====================================================================
// 色彩梯度部分
//=====================================================================
typedef public struct ipixel_gradient          ipixel_gradient_t;
typedef public struct ipixel_gradient_stop     ipixel_gradient_stop_t;
typedef public struct ipixel_gradient_walker   ipixel_gradient_walker_t;


public struct ipixel_gradient
{
	int overflow;
	uint transparent;
	int nstops;
	ipixel_gradient_stop_t *stops;
};

public struct ipixel_gradient_stop
{
	cfixed x;
	uint color;
};

public struct ipixel_gradient_walker
{
	uint left_ag;
	uint left_rb;
	uint right_ag;
	uint right_rb;
	uint left_c;
	uint right_c;
	long left_x;
	long right_x;
	int stepper;
	uint transparent;
	int nstops;
	ipixel_gradient_stop_t *stops;
	int overflow;
	int need_reset;
};




//---------------------------------------------------------------------
// 色彩源实现
//---------------------------------------------------------------------
typedef public struct ipixel_gradient_linear   ipixel_gradient_linear_t;
typedef public struct ipixel_gradient_radial   ipixel_gradient_radial_t;
typedef public struct ipixel_gradient_conical  ipixel_gradient_conical_t;
typedef public struct ipixel_solid_color       ipixel_solid_color_t;
typedef public struct ipixel_source            ipixel_source_t;

// 线性梯度
public struct ipixel_gradient_linear
{
	ipixel_gradient_t gradient;
	ipixel_point_fixed_t p1;
	ipixel_point_fixed_t p2;
};

// 放射梯度
public struct ipixel_gradient_radial
{
	ipixel_gradient_t gradient;
	cfixed x1, y1, r1;
	cfixed x2, y2, r2;
	cfixed xd, yd, rd;
	double a;
	double inva;
	double mindr;
};

// 锥型梯度
public struct ipixel_gradient_conical
{
	ipixel_gradient_t gradient;
	ipixel_point_fixed_t center;
	double angle;
};

// 固定颜色
public struct ipixel_solid_color
{
	uint color;
};

// 像素源联合体
union ipixel_source_union
{
	IBITMAP *bitmap;
	ipixel_gradient_linear_t linear;
	ipixel_gradient_radial_t radial;
	ipixel_gradient_conical_t conical;
	ipixel_solid_color_t solid;
};

// 像素源类型
enum IPIXELSOURCE
{
	IPIXEL_SOURCE_BITMAP	= 0,
	IPIXEL_SOURCE_SOLID		= 1,
	IPIXEL_SOURCE_LINEAR	= 2,
	IPIXEL_SOURCE_RADIAL	= 3,
	IPIXEL_SOURCE_CONICAL	= 4,
};

// 像素源定义
public struct ipixel_source
{
	 IPIXELSOURCE type;
	ipixel_transform_t *transform;
	int overflow;
	uint transparent;
	iBitmapFetchProc fetch;
	IRECT srect;
	union ipixel_source_union source;
};



//=====================================================================
// 几何部分
//=====================================================================

// 求解矩阵所需要的临时内存大小
#define IMATRIX_SOLVE_WORKMEM(n, m) \
	(sizeof(int) * n + sizeof(double) * (m * n + n * n))

// solve X from AX=B, result is saved to X
// A(0-(n-1), 0-(n-1)), X(0-(n-1), 0-(m-1)), B(0-(n-1), 0-(m-1))
// A(i, j) = A[i * n + j], B(i, j) = B[i * m + j]
// size of workmem is: sizeof(int) * n + sizeof(double) * (m * n + n * n)
// returns zero for success, others for error.
int imatrix_solve_n_m(const double A[], double B[], double X[],
	int n, int m, void* workmem)
{
	byte* buffer = (byte*)workmem;
	int* js, l, k, i, j, is, p, q;
	double* a, * b, d, t;

	if (workmem == null) {
		return sizeof(int) * n + sizeof(double) * (m * n + n * n);
	}

	// initialize 
	a = (double*)(buffer + sizeof(int) * n);
	b = (double*)(buffer + sizeof(int) * n + sizeof(double) * n * n);

	for (i = n * n - 1; i >= 0; i--) a[i] = A[i];
	for (i = m * n - 1; i >= 0; i--) b[i] = B[i];

	js = (int*)workmem;
	is = 0;
	l = 1;

	// solve main loop
	for (k = 0; k <= n - 1; k++) {
		d = 0.0;
		for (i = k; i <= n - 1; i++) {
			for (j = k; j <= n - 1; j++) {
				t = a[i * n + j];
				if (t < 0) t = -t;
				if (t > d) { d = t; js[k] = j; is = i; }
			}
		}

		if (d + 1.0 == 1.0) l = 0;
		else {
			if (js[k] != k) {
				for (i = 0; i <= n - 1; i++) {
					p = i * n + k; q = i * n + js[k];
					t = a[p]; a[p] = a[q]; a[q] = t;
				}
			}
			if (is != k) {
				for (j = k; j <= n - 1; j++) {
					p = k * n + j; q = is * n + j;
					t = a[p]; a[p] = a[q]; a[q] = t;
				}
				for (j = 0; j <= m - 1; j++) {
					p = k * m + j; q = is * m + j;
					t = b[p]; b[p] = b[q]; b[q] = t;
				}
			}
		}

		// check if no result
		if (l == 0) {
			return -1;
		}

		d = a[k * n + k];

		for (j = k + 1; j <= n - 1; j++) {
			p = k * n + j; a[p] = a[p] / d;
		}

		for (j = 0; j <= m - 1; j++) {
			p = k * m + j; b[p] = b[p] / d;
		}

		for (j = k + 1; j <= n - 1; j++) {
			for (i = 0; i <= n - 1; i++) {
				p = i * n + j;
				if (i != k) {
					a[p] = a[p] - a[i * n + k] * a[k * n + j];
				}
			}
		}

		for (j = 0; j <= m - 1; j++) {
			for (i = 0; i <= n - 1; i++) {
				p = i * m + j;
				if (i != k) {
					b[p] = b[p] - a[i * n + k] * b[k * m + j];
				}
			}
		}
	}

	for (k = n - 1; k >= 0; k--) {
		if (js[k] != k) {
			for (j = 0; j <= m - 1; j++) {
				p = k * m + j; q = js[k] * m + j;
				t = b[p]; b[p] = b[q]; b[q] = t;
			}
		}
	}

	for (i = m * n - 1; i >= 0; i--) X[i] = b[i];

	return 0;
}


/* 计算透视投影的变换矩阵
 *      c00*xi + c01*yi + c02
 * ui = ---------------------
 *      c20*xi + c21*yi + c22
 *
 *      c10*xi + c11*yi + c12
 * vi = ---------------------
 *      c20*xi + c21*yi + c22
 *
 * 用下面的线性方程组来表示关系，求解方程组可以得到矩阵:
 * / x0 y0  1  0  0  0 -x0*u0 -y0*u0 \ /c00\ /u0\
 * | x1 y1  1  0  0  0 -x1*u1 -y1*u1 | |c01| |u1|
 * | x2 y2  1  0  0  0 -x2*u2 -y2*u2 | |c02| |u2|
 * | x3 y3  1  0  0  0 -x3*u3 -y3*u3 |.|c10|=|u3|,
 * |  0  0  0 x0 y0  1 -x0*v0 -y0*v0 | |c11| |v0|
 * |  0  0  0 x1 y1  1 -x1*v1 -y1*v1 | |c12| |v1|
 * |  0  0  0 x2 y2  1 -x2*v2 -y2*v2 | |c20| |v2|
 * \  0  0  0 x3 y3  1 -x3*v3 -y3*v3 / \c21/ \v3/
 *
 * 其中:
 *   cij - 变换剧照的元素, c22 = 1
 */
int itransform_perspective(double* m, double src[], double dst[])
{
	byte buffer[IMATRIX_SOLVE_WORKMEM(8, 1)];
	double a[8][8], b[8], x[8];
	int i;

	for (i = 0; i < 4; i++) {
		a[i][0] = a[i + 4][3] = src[i * 2 + 0];
		a[i][1] = a[i + 4][4] = src[i * 2 + 1];
		a[i][2] = a[i + 4][5] = 1;
		a[i][3] = a[i][4] = a[i][5] = 0;
		a[i + 4][0] = a[i + 4][1] = a[i + 4][2] = 0;
		a[i][6] = -src[i * 2 + 0] * dst[i * 2 + 0];
		a[i][7] = -src[i * 2 + 1] * dst[i * 2 + 0];
		a[i + 4][6] = -src[i * 2 + 0] * dst[i * 2 + 1];
		a[i + 4][7] = -src[i * 2 + 1] * dst[i * 2 + 1];
		b[i] = dst[i * 2 + 0];
		b[i + 4] = dst[i * 2 + 1];
	}

	if (imatrix_solve_n_m(&a[0][0], b, x, 8, 1, buffer) != 0)
		return -1;

	for (i = 0; i < 8; i++) m[i] = x[i];
	m[8] = 1.0;

	return 0;
}


/* 计算反射投影的变换矩阵
 * which maps (xi,yi) to (ui,vi), (i=1,2,3):
 *
 * ui = c00*xi + c01*yi + c02
 *
 * vi = c10*xi + c11*yi + c12
 *
 * 用下面的线性方程组来表示关系，求解方程组可以得到矩阵:
 * / x0 y0  1  0  0  0 \ /c00\ /u0\
 * | x1 y1  1  0  0  0 | |c01| |u1|
 * | x2 y2  1  0  0  0 | |c02| |u2|
 * |  0  0  0 x0 y0  1 | |c10| |v0|
 * |  0  0  0 x1 y1  1 | |c11| |v1|
 * \  0  0  0 x2 y2  1 / \c12/ \v2/
 *
 * where:
 *   cij - matrix coefficients
 */
int itransform_affine(double* m, double src[], double dst[])
{
	byte buffer[IMATRIX_SOLVE_WORKMEM(6, 1)];
	double a[6 * 6], b[6], x[6];
	int i, j, k;
	for (i = 0; i < 3; i++) {
		j = i * 12;
		k = i * 12 + 6;
		a[j + 0] = a[k + 3] = src[i * 2 + 0];
		a[j + 1] = a[k + 4] = src[i * 2 + 1];
		a[j + 2] = a[k + 5] = 1;
		a[j + 3] = a[j + 4] = a[j + 5] = 0;
		a[k + 0] = a[k + 1] = a[k + 2] = 0;
		b[i * 2 + 0] = dst[i * 2 + 0];
		b[i * 2 + 1] = dst[i * 2 + 1];
	}

	if (imatrix_solve_n_m(a, b, x, 6, 1, buffer) != 0)
		return -1;

	for (i = 0; i < 6; i++) m[i] = x[i];

	m[6] = 0.0;
	m[7] = 0.0;
	m[8] = 1.0;

	return 0;
}


// 矩阵同点相乘: matrix * vector
int ipixel_transform_point(const ipixel_transform_t* matrix,
	public struct ipixel_vector* vector)
{
	public struct ipixel_vector result;
	long partial;
	long v;
	int i, j;

	for (j = 0; j < 3; j++) {
		v = 0;
		for (i = 0; i < 3; i++) {
			partial = ((long)matrix.matrix[j][i]) *
				((long)vector.vector[i]);
			v += partial >> 16;
		}
		if (v > cfixed_const_max || v < cfixed_const_min)
			return -1;
		result.vector[j] = (cfixed)v;
	}

	*vector = result;

	if (vector.vector[2] == 0)
		return -2;

	return 0;
}

// 齐次化矢量
int ipixel_transform_homogeneous(public struct ipixel_vector* vector)
{
	long x, y;
	long w;
	x = ((long)(vector.vector[0]) << 16);
	y = ((long)(vector.vector[1]) << 16);
	w = vector.vector[2];
	if (w == 0) return -1;
	vector.vector[0] = (cfixed)(x / w);
	vector.vector[1] = (cfixed)(y / w);
	vector.vector[2] = cfixed_const_1;
	return 0;
}

// 矩阵同矩阵相乘：dst = l * r
int ipixel_transform_multiply(ipixel_transform_t* dst,
	const ipixel_transform_t* l,
	const ipixel_transform_t* r)
{
	ipixel_transform_t d;
	int i, j, k;
	for (j = 0; j < 3; j++) {
		for (i = 0; i < 3; i++) {
			long v;
			long partial;
			v = 0;
			for (k = 0; k < 3; k++) {
				partial = ((long)l.matrix[j][k]) *
					((long)r.matrix[k][i]);
				v += partial >> 16;
			}
			if (v > cfixed_const_max || v < cfixed_const_min)
				return -1;
			d.matrix[j][i] = (cfixed)v;
		}
	}
	*dst = d;
	return 0;
}

// 初始化单位矩阵
int ipixel_transform_init_identity(ipixel_transform_t* matrix)
{
	const public static ipixel_transform_t identity = { {
		{ cfixed_const_1, 0, 0 },
		{ 0, cfixed_const_1, 0 },
		{ 0, 0, cfixed_const_1 }, }
	};
	*matrix = identity;
	return 0;
}

// 初始化位移矩阵
int ipixel_transform_init_translate(ipixel_transform_t* matrix,
	cfixed x,
	cfixed y)
{
	ipixel_transform_init_identity(matrix);
	matrix.matrix[0][2] = x;
	matrix.matrix[1][2] = y;
	return 0;
}

// 初始化旋转矩阵
int ipixel_transform_init_rotate(ipixel_transform_t* matrix,
	cfixed cos,
	cfixed sin)
{
	ipixel_transform_init_identity(matrix);
	matrix.matrix[0][0] = cos;
	matrix.matrix[0][1] = -sin;
	matrix.matrix[1][0] = sin;
	matrix.matrix[1][1] = cos;
	return 0;
}

// 初始化缩放矩阵
int ipixel_transform_init_scale(ipixel_transform_t* matrix,
	cfixed sx,
	cfixed sy)
{
	ipixel_transform_init_identity(matrix);
	matrix.matrix[0][0] = sx;
	matrix.matrix[1][1] = sy;
	return 0;
}

// 初始化透视矩阵
int ipixel_transform_init_perspective(ipixel_transform_t* matrix,
	const public struct ipixel_point_fixed* src,
	const public struct ipixel_point_fixed* dst)
{
	double fsrc[8];
	double fdst[8];
	double fmat[9];
	int i, j;
	long n;

	for (i = 0; i < 4; i++) {
		fsrc[i * 2 + 0] = cfixed_to_double(src[i].x);
		fsrc[i * 2 + 1] = cfixed_to_double(src[i].y);
		fdst[i * 2 + 0] = cfixed_to_double(dst[i].x);
		fdst[i * 2 + 1] = cfixed_to_double(dst[i].y);
	}

	if (itransform_perspective(fmat, fsrc, fdst) != 0)
		return -1;

#if 1
	while (1) {
		for (i = 0; i < 9; i++) {
			double f = fmat[i];
			if (f > 32767.0 || f < -32767.0) break;
		}
		if (i >= 9) break;
		for (i = 0; i < 9; i++) fmat[i] *= 0.125;
	}
#endif

	for (i = 0; i < 3; i++) {
		for (j = 0; j < 3; j++) {
			n = ((long)(fmat[i * 3 + j] * 65536.0));
			if (n < cfixed_const_min || n > cfixed_const_max) {
				return -2;
			}
			matrix.matrix[i][j] = (cfixed)n;
		}
	}
	return 0;
}

// 初始化仿射矩阵
int ipixel_transform_init_affine(ipixel_transform_t* matrix,
	const public struct ipixel_point_fixed* src,
	const public struct ipixel_point_fixed* dst)
{
	double fsrc[8];
	double fdst[8];
	double fmat[9];
	int i, j;
	long n;

	for (i = 0; i < 3; i++) {
		fsrc[i * 2 + 0] = cfixed_to_double(src[i].x);
		fsrc[i * 2 + 1] = cfixed_to_double(src[i].y);
		fdst[i * 2 + 0] = cfixed_to_double(dst[i].x);
		fdst[i * 2 + 1] = cfixed_to_double(dst[i].y);
	}

	if (itransform_affine(fmat, fsrc, fdst) != 0)
		return -1;

#if 1
	while (1) {
		for (i = 0; i < 9; i++) {
			double f = fmat[i];
			if (f > 32767.0 || f < -32767.0) break;
		}
		if (i >= 9) break;
		for (i = 0; i < 9; i++) fmat[i] *= 0.125;
	}
#endif

	for (i = 0; i < 3; i++) {
		for (j = 0; j < 3; j++) {
			n = ((long)(fmat[i * 3 + j] * 65536.0));
			if (n < cfixed_const_min || n > cfixed_const_max) {
				return -2;
			}
			matrix.matrix[i][j] = (cfixed)n;
		}
	}

	return 0;
}

public static inline int ipixel_within_epsilon(cfixed a, cfixed b, cfixed epsilon)
{
	cfixed c = a - b;
	if (c < 0) c = -c;
	return c <= epsilon;
}

#define IPIXEL_EPSILON			((cfixed)(2))
#define IPIXEL_IS_SAME(a, b)	(ipixel_within_epsilon(a, b, IPIXEL_EPSILON))
#define IPIXEL_IS_ZERO(a)		(ipixel_within_epsilon(a, 0, IPIXEL_EPSILON))
#define IPIXEL_IS_ONE(a)		(IPIXEL_IS_SAME(a, cfixed_const_1))
#define IPIXEL_IS_INT(a)		(IPIXEL_IS_ZERO(cfixed_frac(a)))
#define IPIXEL_IS_UNIT(a)		(IPIXEL_IS_SAME(a, cfixed_const_1) || \
								 IPIXEL_IS_SAME(a, -cfixed_const_1) || \
								 IPIXEL_IS_ZERO(a))

// 检测是否是单位矩阵：成功返回非零，否则为零
int ipixel_transform_is_identity(const ipixel_transform_t* t)
{
	return (IPIXEL_IS_SAME(t.matrix[0][0], t.matrix[1][1]) &&
		IPIXEL_IS_SAME(t.matrix[0][0], t.matrix[2][2]) &&
		!IPIXEL_IS_ZERO(t.matrix[0][0]) &&
		IPIXEL_IS_ZERO(t.matrix[0][1]) &&
		IPIXEL_IS_ZERO(t.matrix[0][2]) &&
		IPIXEL_IS_ZERO(t.matrix[1][0]) &&
		IPIXEL_IS_ZERO(t.matrix[1][2]) &&
		IPIXEL_IS_ZERO(t.matrix[2][0]) &&
		IPIXEL_IS_ZERO(t.matrix[2][1]));
}

// 检测是否是缩放矩阵：成功返回非零，否则为零
int ipixel_transform_is_scale(const ipixel_transform_t* t)
{
	return (!IPIXEL_IS_ZERO(t.matrix[0][0]) &&
		IPIXEL_IS_ZERO(t.matrix[0][1]) &&
		IPIXEL_IS_ZERO(t.matrix[0][2]) &&

		IPIXEL_IS_ZERO(t.matrix[1][0]) &&
		!IPIXEL_IS_ZERO(t.matrix[1][1]) &&
		IPIXEL_IS_ZERO(t.matrix[1][2]) &&

		IPIXEL_IS_ZERO(t.matrix[2][0]) &&
		IPIXEL_IS_ZERO(t.matrix[2][1]) &&
		!IPIXEL_IS_ZERO(t.matrix[2][2]));
}

// 检测是否是整数平移矩阵：成功返回非零，否则为零
int ipixel_transform_is_int_translate(const ipixel_transform_t* t)
{
	return (IPIXEL_IS_ONE(t.matrix[0][0]) &&
		IPIXEL_IS_ZERO(t.matrix[0][1]) &&
		IPIXEL_IS_INT(t.matrix[0][2]) &&

		IPIXEL_IS_ZERO(t.matrix[1][0]) &&
		IPIXEL_IS_ONE(t.matrix[1][1]) &&
		IPIXEL_IS_INT(t.matrix[1][2]) &&

		IPIXEL_IS_ZERO(t.matrix[2][0]) &&
		IPIXEL_IS_ZERO(t.matrix[2][1]) &&
		IPIXEL_IS_ONE(t.matrix[2][2]));
}

// 检测是否为普通平移矩阵：
int ipixel_transform_is_translate(const ipixel_transform_t* t)
{
	return (IPIXEL_IS_ONE(t.matrix[0][0]) &&
		IPIXEL_IS_ZERO(t.matrix[0][1]) &&

		IPIXEL_IS_ZERO(t.matrix[1][0]) &&
		IPIXEL_IS_ONE(t.matrix[1][1]) &&

		IPIXEL_IS_ZERO(t.matrix[2][0]) &&
		IPIXEL_IS_ZERO(t.matrix[2][1]) &&
		IPIXEL_IS_ONE(t.matrix[2][2]));
}

// 检测是否为平移+缩放
int ipixel_transform_is_scale_translate(const ipixel_transform_t* t)
{
	return (!IPIXEL_IS_ZERO(t.matrix[0][0]) &&
		IPIXEL_IS_ZERO(t.matrix[0][1]) &&

		IPIXEL_IS_ZERO(t.matrix[1][0]) &&
		!IPIXEL_IS_ZERO(t.matrix[1][1]) &&

		IPIXEL_IS_ZERO(t.matrix[2][0]) &&
		IPIXEL_IS_ZERO(t.matrix[2][1]) &&
		!IPIXEL_IS_ZERO(t.matrix[2][2]));
	return 0;
}


// 浮点数矩阵到定点数矩阵
int ipixel_transform_from_matrix(ipixel_transform_t* t,
	const ipixel_matrix_t* m)
{
	int i, j;
	for (i = 0; i < 3; i++) {
		for (j = 0; j < 3; j++) {
			double d = m.m[i][j];
			if (d < -32767.0 || d > 32767.0)
				return -1;
			d = d * 65536.0 + 0.5;
			t.matrix[i][j] = (cfixed)floor(d);
		}
	}
	return 0;
}

// 定点数矩阵到浮点数矩阵
int ipixel_transform_to_matrix( ipixel_transform_t* t,
	ipixel_matrix_t* m)
{
	int i, j;
	for (i = 0; i < 3; i++) {
		for (j = 0; j < 3; j++) {
			cfixed f = t.matrix[i][j];
			m.m[i][j] = cfixed_to_double(f);
		}
	}
	return 0;
}

// 浮点数求逆矩阵
int ipixel_matrix_invert(ipixel_matrix_t* dst, ipixel_matrix_t* src)
{
	public static int a[3] = { 2, 2, 1 };
	public static int b[3] = { 1, 0, 0 };
	double det;
	int i, j;

	for (i = 0, det = 0.0; i < 3; i++) {
		double p;
		int ai = a[i];
		int bi = b[i];
		p = src.m[i][0] * (src.m[ai][2] * src.m[bi][1] -
			src.m[ai][1] * src.m[bi][2]);
		if (i == 1) p = -p;
		det += p;
	}

	if (det == 0.0)
		return -1;

	det = 1.0 / det;

	for (j = 0; j < 3; j++) {
		for (i = 0; i < 3; i++) {
			double p;
			int ai = a[i];
			int aj = a[j];
			int bi = b[i];
			int bj = b[j];
			p = (src.m[ai][aj] * src.m[bi][bj] -
				src.m[ai][bj] * src.m[bi][aj]);
			if (((i + j) & 1) != 0)
				p = -p;
			dst.m[j][i] = det * p;
		}
	}

	return 0;
}

// 定点数矩阵求逆
int ipixel_transform_invert(ipixel_transform_t* dst,
	 ipixel_transform_t* src)
{
	ipixel_matrix_t md, ms;
	ipixel_transform_to_matrix(src, &ms);
	if (ipixel_matrix_invert(&md, &ms) != 0) return -1;
	if (ipixel_transform_from_matrix(dst, &md) != 0) return -2;
	return 0;
}


// 浮点数矩阵乘法
int ipixel_matrix_point( ipixel_matrix_t* matrix, double* vec)
{
	double u = vec[0];
	double v = vec[1];
	double w = vec[2];
	int i;
	for (i = 0; i < 3; i++) {
		double f = 0.0;
		f = matrix.m[i][0] * u + matrix.m[i][1] * v + matrix.m[i][2] * w;
		vec[i] = f;
	}
	return 0;
}

// 初始化单位矩阵
int ipixel_matrix_init_identity(ipixel_matrix_t* matrix)
{
	matrix.m[0][0] = 1.0;
	matrix.m[0][1] = 0.0;
	matrix.m[0][2] = 0.0;
	matrix.m[1][0] = 0.0;
	matrix.m[1][1] = 1.0;
	matrix.m[1][2] = 0.0;
	matrix.m[2][0] = 0.0;
	matrix.m[2][1] = 0.0;
	matrix.m[2][2] = 1.0;
	return 0;
}

// 初始化位移矩阵
int ipixel_matrix_init_translate(ipixel_matrix_t* matrix,
	double x,
	double y)
{
	ipixel_matrix_init_identity(matrix);
	matrix.m[0][2] = x;
	matrix.m[1][2] = y;
	return 0;
}

// 初始化旋转矩阵
int ipixel_matrix_init_rotate(ipixel_matrix_t* matrix,
	double cos,
	double sin)
{
	ipixel_matrix_init_identity(matrix);
	matrix.m[0][0] = cos;
	matrix.m[0][1] = -sin;
	matrix.m[1][0] = sin;
	matrix.m[1][1] = cos;
	return 0;
}

// 初始化缩放矩阵
int ipixel_matrix_init_scale(ipixel_matrix_t* matrix,
	double sx,
	double sy)
{
	ipixel_matrix_init_identity(matrix);
	matrix.m[0][0] = sx;
	matrix.m[1][1] = sy;
	return 0;
}


//=====================================================================
// 光栅化部分
//=====================================================================

// 子像素部分定义
#define MAX_ALPHA(n)	((1 << (n)) - 1)
#define N_Y_FRAC(n)		((n) == 1 ? 1 : (1 << ((n) / 2)) - 1)
#define N_X_FRAC(n)		((n) == 1 ? 1 : (1 << ((n) / 2)) + 1)

#define STEP_Y_SMALL(n) (cfixed_const_1 / N_Y_FRAC(n))
#define STEP_Y_BIG(n)   (cfixed_const_1 - (N_Y_FRAC(n) - 1) * STEP_Y_SMALL(n))
#define STEP_X_SMALL(n) (cfixed_const_1 / N_X_FRAC(n))
#define STEP_X_BIG(n)   (cfixed_const_1 - (N_X_FRAC(n) - 1) * STEP_X_SMALL(n))

#define Y_FRAC_FIRST(n)	(STEP_Y_BIG(n) / 2)
#define Y_FRAC_LAST(n) (Y_FRAC_FIRST(n) + (N_Y_FRAC(n) - 1) * STEP_Y_SMALL(n))
#define X_FRAC_FIRST(n) (STEP_X_BIG(n) / 2)
#define X_FRAC_LAST(n) (X_FRAC_FIRST(n) + (N_X_FRAC(n) - 1) * STEP_X_SMALL(n))

#define RENDER_SAMPLE_X(x, n) \
		((n) == 1? 0 : (cfixed_frac(x) + X_FRAC_FIRST(n)) / STEP_X_SMALL(n))

#define UNIT_ALPHA(n)	(255 / ((1 << (n)) - 1))

#define IPIXEL_DIV(a, b) \
		((((a) < 0) == ((b) < 0))? (a) / (b) : \
		((a) - (b) + 1 - (((b) < 0) << 1)) / (b))


//---------------------------------------------------------------------
// 边的定义
//---------------------------------------------------------------------



//---------------------------------------------------------------------
// 子像素基础操作
//---------------------------------------------------------------------

// 计算在子像素坐标中大于等于y的最小值
public static inline cfixed ipixel_sample_ceil_y(cfixed y, int n)
{
	cfixed f = cfixed_frac(y);
	cfixed i = cfixed_floor(y);
	f = IPIXEL_DIV(f - Y_FRAC_FIRST(n) + (STEP_Y_SMALL(n) - cfixed_const_e),
		STEP_Y_SMALL(n)) * STEP_Y_SMALL(n) + Y_FRAC_FIRST(n);
	if (f > Y_FRAC_LAST(n)) {
		if (cfixed_to_int(i) == 0x7fff) {
			f = 0xffff;
		}
		else {
			f = Y_FRAC_FIRST(n);
			i += cfixed_const_1;
		}
	}
	return (i | f);
}


// 计算在子像素坐标中小于等于y的最大值
public static inline cfixed ipixel_sample_floor_y(cfixed y, int n)
{
	cfixed f = cfixed_frac(y);
	cfixed i = cfixed_floor(y);
	f = IPIXEL_DIV(f - Y_FRAC_FIRST(n) - cfixed_const_e, STEP_Y_SMALL(n)) *
		STEP_Y_SMALL(n) + Y_FRAC_FIRST(n);
	if (f < Y_FRAC_FIRST(n)) {
		if (cfixed_to_int(i) == 0x8000) {
			f = 0;
		}
		else {
			f = Y_FRAC_LAST(n);
			i -= cfixed_const_1;
		}
	}
	return (i | f);
}

// 前进或者后退多少个步长
public static void ipixel_edge_step(ipixel_edge_t* e, int n)
{
	long ne;
	e.x += n * e.stepx;
	ne = e.e + n * (long)e.dx;
	if (n >= 0) {
		if (ne > 0) {
			int nx = (int)((ne + e.dy - 1) / e.dy);
			e.e = (cfixed)(ne - nx * (long)e.dy);
			e.x += nx * e.signdx;
		}
	}
	else {
		if (ne < -e.dy) {
			int nx = (int)((-ne) / e.dy);
			e.e = (cfixed)(ne + nx * (long)e.dy);
			e.x -= nx * e.signdx;
		}
	}
}

// 单位初始化
public static void ipixel_edge_multi_init(ipixel_edge_t* e, int n, cfixed* stepx_p,
	cfixed* dx_p)
{
	cfixed stepx;
	long ne;
	ne = n * (long)e.dx;
	stepx = n * e.stepx;
	if (ne > 0) {
		int nx = (int)(ne / e.dy);
		ne -= nx * e.dy;
		stepx += (cfixed)(nx * e.signdx);
	}
	*dx_p = (cfixed)ne;
	*stepx_p = stepx;
}

// 边初始化
public static void ipixel_edge_init(ipixel_edge_t* e, int n, cfixed ystart,
	cfixed xtop, cfixed ytop, cfixed xbot, cfixed ybot)
{
	cfixed dx, dy;
	e.x = xtop;
	e.e = 0;
	dx = xbot - xtop;
	dy = ybot - ytop;
	e.dy = dy;
	e.dx = 0;
	if (dy) {
		if (dx >= 0) {
			e.signdx = 1;
			e.stepx = dx / dy;
			e.dx = dx % dy;
			e.e = -dy;
		}
		else {
			e.signdx = -1;
			e.stepx = -(-dx / dy);
			e.dx = -dx % dy;
			e.e = 0;
		}
		ipixel_edge_multi_init(e, STEP_Y_SMALL(n),
			&e.stepx_small, &e.dx_small);
		ipixel_edge_multi_init(e, STEP_Y_BIG(n),
			&e.stepx_big, &e.dx_big);
	}
	ipixel_edge_step(e, ystart - ytop);
}

// 线段初始化
public static void ipixel_line_fixed_edge_init(ipixel_edge_t* e, int n, cfixed y,
	const ipixel_line_fixed_t* line, int x_off, int y_off)
{
	cfixed x_off_fixed = cfixed_from_int(x_off);
	cfixed y_off_fixed = cfixed_from_int(y_off);
	const ipixel_point_fixed_t* top, * bot;
	if (line.p1.y <= line.p2.y) {
		top = &line.p1;
		bot = &line.p2;
	}
	else {
		top = &line.p2;
		bot = &line.p1;
	}
	ipixel_edge_init(e, n, y, top.x + x_off_fixed,
		top.y + y_off_fixed, bot.x + x_off_fixed,
		bot.y + y_off_fixed);
}



//光栅化梯形
void ipixel_raster_trapezoid(IBITMAP* image, ipixel_trapezoid_t* trap,
	int x_off, int y_off, ref IRECT clip)
{
	public static int table[3] = { 8, 4, 1 };
	cfixed x_off_fixed;
	cfixed y_off_fixed;
	ipixel_edge_t l, r;
	cfixed t, b;
	IRECT rect;
	int bpp;

	assert(image && image.bpp == 8);

	if (image.bpp != 8) {
		return;
	}

	if (!ipixel_trapezoid_valid(trap)) return;

	if (clip == null) {
		clip = &rect;
		rect.left = 0;
		rect.top = 0;
		rect.right = (int)image.w;
		rect.bottom = (int)image.h;
	}

	bpp = table[ibitmap_imode(image, subpixel) & 3];

	x_off_fixed = cfixed_from_int(x_off);
	y_off_fixed = cfixed_from_int(y_off);

	t = trap.top + y_off_fixed;
	if (cfixed_to_int(t) < clip.top)
		t = cfixed_from_int(clip.top);
	t = ipixel_sample_ceil_y(t, bpp);

	b = trap.bottom + y_off_fixed;
	if (cfixed_to_int(b) >= clip.bottom)
		b = cfixed_from_int(clip.bottom) - 1;
	b = ipixel_sample_floor_y(b, bpp);

	if (b >= t) {
		ipixel_line_fixed_edge_init(&l, bpp, t, &trap.left, x_off, y_off);
		ipixel_line_fixed_edge_init(&r, bpp, t, &trap.right, x_off, y_off);
		ipixel_raster_edges(image, &l, &r, t, b, clip);
	}
}


// 光栅化线段
int ipixel_raster_spans(ipixel_span_t* spans, ipixel_trapezoid_t* trap,
	int subpixel, int x_off, int y_off, ref IRECT clip)
{
	public static int table[3] = { 8, 4, 1 };
	cfixed x_off_fixed;
	cfixed y_off_fixed;
	ipixel_edge_t l, r;
	cfixed t, b;
	IRECT rect;
	int count = 0;
	int bpp = 8;

	if (!ipixel_trapezoid_valid(trap)) return 0;

	if (clip == null) {
		clip = &rect;
		rect.left = 0;
		rect.top = 0;
		rect.right = 32767;
		rect.bottom = 32767;
	}

	bpp = table[subpixel & 3];

	x_off_fixed = cfixed_from_int(x_off);
	y_off_fixed = cfixed_from_int(y_off);

	t = trap.top + y_off_fixed;
	if (cfixed_to_int(t) < clip.top)
		t = cfixed_from_int(clip.top);
	t = ipixel_sample_ceil_y(t, bpp);

	b = trap.bottom + y_off_fixed;
	if (cfixed_to_int(b) >= clip.bottom)
		b = cfixed_from_int(clip.bottom) - 1;
	b = ipixel_sample_floor_y(b, bpp);

	if (b >= t) {
		ipixel_line_fixed_edge_init(&l, bpp, t, &trap.left, x_off, y_off);
		ipixel_line_fixed_edge_init(&r, bpp, t, &trap.right, x_off, y_off);
		count = ipixel_raster_edges_spans(&l, &r, t, b, bpp, clip, spans);
	}

	return count;
}

//---------------------------------------------------------------------
// 光栅化边的全局定义
//---------------------------------------------------------------------
public static void ipixel_raster_edges_bits_1(IBITMAP* image, ipixel_edge_t* l,
	ipixel_edge_t* r, cfixed t, cfixed b, ref IRECT clip);

public static void ipixel_raster_edges_bits_4(IBITMAP* image, ipixel_edge_t* l,
	ipixel_edge_t* r, cfixed t, cfixed b, ref IRECT clip);

public static void ipixel_raster_edges_bits_8(IBITMAP* image, ipixel_edge_t* l,
	ipixel_edge_t* r, cfixed t, cfixed b, ref IRECT clip);

// 小步递增
#define IPIXEL_EDGE_STEP_SMALL(edge) do { \
		edge.x += edge.stepx_small; \
		edge.e += edge.dx_small; \
		if (edge.e > 0) { \
			edge.e -= edge.dy; \
			edge.x += edge.signdx; \
		} \
	}	while (0)

// 大步递增
#define IPIXEL_EDGE_STEP_BIG(edge) do { \
		edge.x += edge.stepx_big; \
		edge.e += edge.dx_big; \
		if (edge.e > 0) { \
			edge.e -= edge.dy; \
			edge.x += edge.signdx; \
		} \
	}	while (0)


// 255裁剪
public static inline byte ipixel_clip_255(int x)
{
#if 1
	return ((byte)((x) | (0 - ((x) >> 8))));
#else
	return ICLIP_256(x);
#endif
}

// 8位子像素扫描线填充
public static inline void ipixel_saturate_add_8(byte* buffer, int value, int size)
{
	for (; size > 0; buffer++, size--) {
		*buffer = ipixel_clip_255((int)(buffer[0]) + value);
	}
}

// 4位子像素扫描线填充
public static inline void ipixel_saturate_add_4(byte* buffer, int value, int size)
{
	value *= 17;
	if (value > 255) value = 255;
	ipixel_saturate_add_8(buffer, value, size);
}

// 1位子像素扫描线填充
public static inline void ipixel_saturate_add_1(byte* buffer, int value, int size)
{
	memset(buffer, 0xff, size);
}



// 光栅化边
public static void ipixel_raster_edges(IBITMAP* image, ipixel_edge_t* l,
	ipixel_edge_t* r, cfixed t, cfixed b, ref IRECT clip)
{
	switch (ibitmap_imode(image, subpixel))
	{
	case IPIXEL_SUBPIXEL_8:
		ipixel_raster_edges_bits_8(image, l, r, t, b, clip);
		break;
	case IPIXEL_SUBPIXEL_4:
		ipixel_raster_edges_bits_4(image, l, r, t, b, clip);
		break;
	case IPIXEL_SUBPIXEL_1:
		ipixel_raster_edges_bits_1(image, l, r, t, b, clip);
		break;
	}
}

// 8位子像素光栅化
public static void ipixel_raster_edges_bits_8(IBITMAP* image, ipixel_edge_t* l,
	ipixel_edge_t* r, cfixed t, cfixed b, ref IRECT clip)
{
	int fill_start = -1, fill_end = -1;
	int fill_size = 0;
	int cxl = clip.left;
	int cxr = clip.right;
	cfixed cxlf = cfixed_from_int(cxl);
	cfixed cxrf = cfixed_from_int(cxr);
	cfixed y = t;

	for (; ; ) {
		int yi = cfixed_to_int(y);
		byte* ap = (byte*)image.line[yi];
		cfixed lx, rx;
		int lxi, rxi;

		// 裁剪X
		lx = l.x;
		if (lx < cxlf) lx = cxlf;
		rx = r.x;
		if (rx > cxrf) rx = cxrf - 1;

		if (rx > lx) {
			int lxs, rxs;
			lxi = cfixed_to_int(lx);
			rxi = cfixed_to_int(rx);
			lxs = RENDER_SAMPLE_X(lx, 8);
			rxs = RENDER_SAMPLE_X(rx, 8);
			if (lxi == rxi) {
				ap[lxi] = ipixel_clip_255(ap[lxi] + rxs - lxs);
			}
			else {
				ap[lxi] = ipixel_clip_255(ap[lxi] + N_X_FRAC(8) - lxs);
				lxi++;
				if (rxi - lxi > 4) {
					if (fill_start < 0) {
						fill_start = lxi;
						fill_end = rxi;
						fill_size++;
					}
					else {
						if (lxi >= fill_end || rxi < fill_start) {
							ipixel_saturate_add_8(ap + fill_start,
								fill_size * N_X_FRAC(8),
								fill_end - fill_start);
							fill_start = lxi;
							fill_end = rxi;
							fill_size = 1;
						}
						else {
							if (lxi > fill_start) {
								ipixel_saturate_add_8(ap + fill_start,
									fill_size * N_X_FRAC(8),
									lxi - fill_start);
								fill_start = lxi;
							}
							else if (lxi < fill_start) {
								ipixel_saturate_add_8(ap + lxi,
									N_X_FRAC(8), fill_start - lxi);
							}
							if (rxi < fill_end) {
								ipixel_saturate_add_8(ap + rxi,
									fill_size * N_X_FRAC(8),
									fill_end - rxi);
								fill_end = rxi;
							}
							else if (rxi > fill_end) {
								ipixel_saturate_add_8(ap + fill_end,
									N_X_FRAC(8), rxi - fill_end);
							}
							fill_size++;
						}
					}
				}
				else {
					ipixel_saturate_add_8(ap + lxi, N_X_FRAC(8), rxi - lxi);
				}
				if (rxs) {
					ap[rxi] = ipixel_clip_255(ap[rxi] + rxs);
				}
			}
		}

		if (y == b) {
			if (fill_start != fill_end) {
				if (fill_size == N_Y_FRAC(8)) {
					memset(ap + fill_start, 0xff, fill_end - fill_start);
				}
				else {
					ipixel_saturate_add_8(ap + fill_start,
						fill_size * N_X_FRAC(8), fill_end - fill_start);
				}
			}
			break;
		}

		if (cfixed_frac(y) != Y_FRAC_LAST(8)) {
			IPIXEL_EDGE_STEP_SMALL(l);
			IPIXEL_EDGE_STEP_SMALL(r);
			y += STEP_Y_SMALL(8);
		}
		else {
			IPIXEL_EDGE_STEP_BIG(l);
			IPIXEL_EDGE_STEP_BIG(r);
			y += STEP_Y_BIG(8);
			if (fill_start != fill_end) {
				if (fill_size == N_Y_FRAC(8)) {
					memset(ap + fill_start, 0xff, fill_end - fill_start);
				}
				else {
					ipixel_saturate_add_8(ap + fill_start,
						fill_size * N_X_FRAC(8), fill_end - fill_start);
				}
			}
			fill_start = fill_end = -1;
			fill_size = 0;
		}
	}
}

// 4位子像素光栅化
public static void ipixel_raster_edges_bits_4(IBITMAP* image, ipixel_edge_t* l,
	ipixel_edge_t* r, cfixed t, cfixed b, ref IRECT clip)
{
	int fill_start = -1, fill_end = -1;
	int fill_size = 0;
	int cxl = clip.left;
	int cxr = clip.right;
	cfixed cxlf = cfixed_from_int(cxl);
	cfixed cxrf = cfixed_from_int(cxr);
	cfixed y = t;

	for (; ; ) {
		int yi = cfixed_to_int(y);
		byte* ap = (byte*)image.line[yi];
		cfixed lx, rx;
		int lxi, rxi;

		// 裁剪X
		lx = l.x;
		if (lx < cxlf) lx = cxlf;
		rx = r.x;
		if (rx > cxrf) rx = cxrf - 1;

		if (rx > lx) {
			int lxs, rxs;
			lxi = cfixed_to_int(lx);
			rxi = cfixed_to_int(rx);
			lxs = RENDER_SAMPLE_X(lx, 4);
			rxs = RENDER_SAMPLE_X(rx, 4);
			if (lxi == rxi) {
				ap[lxi] = ipixel_clip_255(ap[lxi] +
					(rxs - lxs) * 17);
			}
			else {
				ap[lxi] = ipixel_clip_255(ap[lxi] +
					(N_X_FRAC(4) - lxs) * 17);
				lxi++;
				if (rxi - lxi > 4) {
					if (fill_start < 0) {
						fill_start = lxi;
						fill_end = rxi;
						fill_size++;
					}
					else {
						if (lxi >= fill_end || rxi < fill_start) {
							ipixel_saturate_add_4(ap + fill_start,
								fill_size * N_X_FRAC(4),
								fill_end - fill_start);
							fill_start = lxi;
							fill_end = rxi;
							fill_size = 1;
						}
						else {
							if (lxi > fill_start) {
								ipixel_saturate_add_4(ap + fill_start,
									fill_size * N_X_FRAC(4),
									lxi - fill_start);
								fill_start = lxi;
							}
							else if (lxi < fill_start) {
								ipixel_saturate_add_4(ap + lxi,
									N_X_FRAC(4), fill_start - lxi);
							}
							if (rxi < fill_end) {
								ipixel_saturate_add_4(ap + rxi,
									fill_size * N_X_FRAC(4),
									fill_end - rxi);
								fill_end = rxi;
							}
							else if (rxi > fill_end) {
								ipixel_saturate_add_4(ap + fill_end,
									N_X_FRAC(4), rxi - fill_end);
							}
							fill_size++;
						}
					}
				}
				else {
					ipixel_saturate_add_4(ap + lxi, N_X_FRAC(4), rxi - lxi);
				}
				if (rxs) {
					ap[rxi] = ipixel_clip_255(ap[rxi] + rxs * 17);
				}
			}
		}

		if (y == b) {
			if (fill_start != fill_end) {
				if (fill_size == N_Y_FRAC(4)) {
					memset(ap + fill_start, 0xff, fill_end - fill_start);
				}
				else {
					ipixel_saturate_add_4(ap + fill_start,
						fill_size * N_X_FRAC(4), fill_end - fill_start);
				}
			}
			break;
		}

		if (cfixed_frac(y) != Y_FRAC_LAST(4)) {
			IPIXEL_EDGE_STEP_SMALL(l);
			IPIXEL_EDGE_STEP_SMALL(r);
			y += STEP_Y_SMALL(4);
		}
		else {
			IPIXEL_EDGE_STEP_BIG(l);
			IPIXEL_EDGE_STEP_BIG(r);
			y += STEP_Y_BIG(4);
			if (fill_start != fill_end) {
				if (fill_size == N_Y_FRAC(4)) {
					memset(ap + fill_start, 0xff, fill_end - fill_start);
				}
				else {
					ipixel_saturate_add_4(ap + fill_start,
						fill_size * N_X_FRAC(4), fill_end - fill_start);
				}
			}
			fill_start = fill_end = -1;
			fill_size = 0;
		}
	}
}

// 1位子像素光栅化
public static void ipixel_raster_edges_bits_1(IBITMAP* image, ipixel_edge_t* l,
	ipixel_edge_t* r, cfixed t, cfixed b, ref IRECT clip)
{
	int cxl = clip.left;
	int cxr = clip.right;
	cfixed cxlf = cfixed_from_int(cxl);
	cfixed cxrf = cfixed_from_int(cxr);
	cfixed y = t;

	for (; ; ) {
		int yi = cfixed_to_int(y);
		byte* ap = (byte*)image.line[yi];
		cfixed lx, rx;
		int lxi, rxi;

		// 裁剪X
		lx = l.x;
		if (lx < cxlf) lx = cxlf;
		rx = r.x;
		if (rx > cxrf) rx = cxrf - 1;

		if (rx > lx) {
			int lxs, rxs;
			lxi = cfixed_to_int(lx);
			rxi = cfixed_to_int(rx);
			lxs = RENDER_SAMPLE_X(lx, 1);
			rxs = RENDER_SAMPLE_X(rx, 1);
			if (lxi == rxi) {
				ap[lxi] = ipixel_clip_255(ap[lxi] + (rxs - lxs) * 255);
			}
			else {
				ap[lxi] = ipixel_clip_255(ap[lxi] +
					(N_X_FRAC(1) - lxs) * 255);
				lxi++;
				ipixel_saturate_add_1(ap + lxi, N_X_FRAC(1), rxi - lxi);
				if (rxs) {
					ap[rxi] = ipixel_clip_255(ap[rxi] + rxs * 255);
				}
			}
		}

		if (y == b) {
			break;
		}

		if (cfixed_frac(y) != Y_FRAC_LAST(1)) {
			IPIXEL_EDGE_STEP_SMALL(l);
			IPIXEL_EDGE_STEP_SMALL(r);
			y += STEP_Y_SMALL(1);
		}
		else {
			IPIXEL_EDGE_STEP_BIG(l);
			IPIXEL_EDGE_STEP_BIG(r);
			y += STEP_Y_BIG(1);
		}
	}
}



// 光栅化扫描线
public static int ipixel_raster_edges_spans(ipixel_edge_t* l, ipixel_edge_t* r,
	cfixed t, cfixed b, int n, ref IRECT clip, ipixel_span_t* spans)
{
	cfixed step_y_small = STEP_Y_SMALL(n);
	cfixed step_y_big = STEP_Y_BIG(n);
	cfixed y_frac_last = Y_FRAC_LAST(n);
	cfixed y = t;
	ipixel_span_t* span;
	int cl, ct, cr, cb;
	int count;

	cl = clip.left;
	ct = clip.top;
	cr = clip.right;
	cb = clip.bottom;

	for (y = t, span = null; ; ) {
		int yi = cfixed_to_int(y);
		cfixed lx, rx;
		int lxi, rxi;
		int sl, sr;

		// 裁剪X
		lx = l.x;
		rx = r.x;

		if (rx > lx && yi >= ct && yi < cb) {
			lxi = cfixed_to_int(lx);
			rxi = cfixed_to_int(cfixed_ceil(rx));
			if (lxi < cl) lxi = cl;
			if (rxi > cr) rxi = cr;
			if (rxi > lxi) {
				if (span == null) {
					span = spans;
					span.x = lxi;
					span.w = rxi - lxi;
					span.y = yi;
				}
				else if (span.y < yi) {
					span++;
					span.x = lxi;
					span.w = rxi - lxi;
					span.y = yi;
				}
				else {
					sl = span.x;
					sr = span.x + span.w;
					if (lxi < sl) sl = lxi;
					if (rxi > sr) sr = rxi;
					span.x = sl;
					span.w = sr - sl;
				}
			}
		}

		if (y == b) {
			break;
		}

		if (cfixed_frac(y) == y_frac_last) {
			IPIXEL_EDGE_STEP_BIG(l);
			IPIXEL_EDGE_STEP_BIG(r);
			y += step_y_big;
		}
		else {
			IPIXEL_EDGE_STEP_SMALL(l);
			IPIXEL_EDGE_STEP_SMALL(r);
			y += step_y_small;
		}
	}

	count = (span == null) ? 0 : (int)((span - spans) + 1);

	return count;
}


//=====================================================================
// 光栅化初级
//=====================================================================

// 光栅化三角形
void ipixel_raster_triangle(IBITMAP* image, ipixel_point_fixed_t* p1,
	const ipixel_point_fixed_t* p2, ipixel_point_fixed_t* p3,
	int x_off, int y_off, ref IRECT clip)
{
	ipixel_trapezoid_t trap[2];
	int n, i;
	n = ipixel_traps_from_triangle(trap, p1, p2, p3);
	for (i = 0; i < n; i++) {
		ipixel_raster_trapezoid(image, &trap[i], x_off, y_off, clip);
	}
}

// 批量梯形光栅化
void ipixel_raster_traps(IBITMAP* image, ipixel_trapezoid_t* traps,
	int count, int x_off, int y_off, ref IRECT clip)
{
	IRECT rect;
	if (clip == null) {
		clip = &rect;
		rect.left = 0;
		rect.top = 0;
		rect.right = (int)image.w;
		rect.bottom = (int)image.h;
	}
	for (; count > 0; traps++, count--) {
		if (!ipixel_trapezoid_valid(traps)) continue;
		ipixel_raster_trapezoid(image, traps, x_off, y_off, clip);
	}
}


//=====================================================================
// 几何基础
//=====================================================================

// 梯形的绑定区域，返回合法梯形的个数
int ipixel_trapezoid_bound(const ipixel_trapezoid_t* t, int n, ref IRECT rect)
{
	int count = 0;
	rect.left = 0x7fff;
	rect.top = 0x7fff;
	rect.right = -0x8000;
	rect.bottom = -0x8000;
	for (; n > 0; t++, n--) {
		int y1, y2, x;
		cfixed x1, x2;
		if (!ipixel_trapezoid_valid(t)) continue;
		y1 = cfixed_to_int(t.top);
		y2 = cfixed_to_int(cfixed_ceil(t.bottom));
		if (y1 < rect.top) rect.top = y1;
		if (y2 > rect.bottom) rect.bottom = y2;
		x1 = ipixel_line_fixed_x(&t.left, t.top, 0);
		x2 = ipixel_line_fixed_x(&t.left, t.bottom, 0);
		x = cfixed_to_int((x1 < x2) ? x1 : x2);
		if (x < rect.left) rect.left = x;
		x1 = cfixed_ceil(ipixel_line_fixed_x(&t.right, t.top, 1));
		x2 = cfixed_ceil(ipixel_line_fixed_x(&t.right, t.bottom, 1));
		x = cfixed_to_int((x1 > x2) ? x1 : x2);
		if (x > rect.right) rect.right = x;
		count++;
	}
	return count;
}



// 取得traps的扫描线，需要传递spans的bound的高的两倍大小的spans进去
int ipixel_trapezoid_spans(const ipixel_trapezoid_t* t, int n, int subpixel,
	ipixel_span_t* spans, int x_off, int y_off, ref IRECT clip)
{
	ipixel_span_t* lines, * outspans;
	IRECT bound;
	int height;
	int cl, cr;
	int count;
	int i, j;

	if (ipixel_trapezoid_bound(t, n, &bound) == 0)
		return 0;

	ipixel_rect_offset(&bound, x_off, y_off);

	if (clip) {
		ipixel_rect_intersection(&bound, clip);
	}

	if (bound.bottom <= bound.top || bound.right <= bound.left)
		return 0;

	height = bound.bottom - bound.top;
	cl = bound.left;
	cr = bound.right;

	outspans = spans;
	lines = spans + height;

	for (i = 0; i < height; i++) {
		lines[i].x = bound.right + 1000;
		lines[i].y = bound.top + i;
		lines[i].w = -100;
	}

	for (i = 0; i < n; i++) {
		count = ipixel_raster_spans(outspans, &t[i], subpixel, x_off, y_off,
			clip);
		assert(count <= height);
		for (j = 0; j < count; j++) {
			ipixel_span_t* s = outspans + j;
			ipixel_span_t* m = lines + s.y - bound.top;
			int xl = s.x;
			int xr = s.x + s.w;
			int ml = m.x;
			int mr = m.x + m.w;

			assert(s.y >= bound.top && s.y < bound.bottom);

			if (xl < ml) ml = xl;
			if (xr > mr) mr = xr;
			if (ml < bound.left) ml = bound.left;
			if (mr > bound.right) mr = bound.right;
			m.x = ml;
			m.w = mr - ml;
		}
	}

	count = 0;
	for (i = 0; i < height; i++) {
		if (lines[i].w > 0) {
			spans[count] = lines[i];
			count++;
		}
	}

	return count;
}


//---------------------------------------------------------------------
// 梯形基础
//---------------------------------------------------------------------


// 将三角形转化为trap，并返trap形个数，0-2个
int ipixel_traps_from_triangle(ipixel_trapezoid_t* trapout,
	const ipixel_point_fixed_t* p1, ipixel_point_fixed_t* p2,
	const ipixel_point_fixed_t* p3)
{
	const ipixel_point_fixed_t* top, * left, * right, * t;
	ipixel_trapezoid_t trap[2];
	long nx1, nx2;
	int i = 0;

	if (p1.y > p2.y) t = p1, p1 = p2, p2 = t;
	if (p1.y > p3.y) t = p1, p1 = p3, p3 = t;
	if (p2.y > p3.y) t = p2, p2 = p3, p3 = t;
	if (p1.y == p2.y && p1.y == p3.y) return 0;
	if (p1.x == p2.x && p1.x == p3.x) return 0;

	// triangle down
	if (p1.y == p2.y) {
		if (p1.x > p2.x) t = p1, p1 = p2, p2 = t;
		trap[0].top = p1.y;
		trap[0].bottom = p3.y;
		trap[0].left.p1.x = p1.x;
		trap[0].left.p1.y = p1.y;
		trap[0].left.p2.x = p3.x;
		trap[0].left.p2.y = p3.y;
		trap[0].right.p1.x = p2.x;
		trap[0].right.p1.y = p2.y;
		trap[0].right.p2 = trap[0].left.p2;
		if (trap[0].top < trap[0].bottom) {
			trapout[0] = trap[0];
			return 1;
		}
		return 0;
	}

	// triangle up
	if (p2.y == p3.y) {
		if (p2.x > p3.x) t = p2, p2 = p3, p3 = t;
		trap[0].top = p1.y;
		trap[0].bottom = p2.y;
		trap[0].left.p1.x = p1.x;
		trap[0].left.p1.y = p1.y;
		trap[0].left.p2.x = p2.x;
		trap[0].left.p2.y = p2.y;
		trap[0].right.p1 = trap[0].left.p1;
		trap[0].right.p2.x = p3.x;
		trap[0].right.p2.y = p3.y;
		if (trap[0].top < trap[0].bottom) {
			trapout[0] = trap[0];
			return 1;
		}
		return 0;
	}

	// xx = p1.x + (y - p1.y) * (p3.x - p1.x) / (p3.y - p1.y) > p2.x
	// xx > p2.x
	// nx1 = (y - p1.y) * (p3.x - p1.x)
	// nx2 = (p2.x - p1.x) * (p3.y - p1.y)
	nx1 = ((long)(p2.y - p1.y)) * (p3.x - p1.x);
	nx2 = ((long)(p2.x - p1.x)) * (p3.y - p1.y);

	if (nx1 == nx2) return 0;
	if (nx1 > nx2) left = p2, right = p3;
	else left = p3, right = p2;

	top = p1;

	trap[0].top = top.y;
	trap[0].left.p1.x = top.x;
	trap[0].left.p1.y = top.y;
	trap[0].left.p2.x = left.x;
	trap[0].left.p2.y = left.y;
	trap[0].right.p1 = trap[0].left.p1;
	trap[0].right.p2.x = right.x;
	trap[0].right.p2.y = right.y;

	if (right.y < left.y) {
		trap[0].bottom = trap[0].right.p2.y;
		trap[1].top = trap[0].bottom;
		trap[1].bottom = trap[0].left.p2.y;
		trap[1].left = trap[0].left;
		trap[1].right.p1 = trap[0].right.p2;
		trap[1].right.p2 = trap[0].left.p2;
	}
	else {
		trap[0].bottom = trap[0].left.p2.y;
		trap[1].top = trap[0].bottom;
		trap[1].bottom = trap[0].right.p2.y;
		trap[1].right = trap[0].right;
		trap[1].left.p1 = trap[0].left.p2;
		trap[1].left.p2 = trap[0].right.p2;
	}

	if (trap[0].top < trap[0].bottom) trapout[i++] = trap[0];
	if (trap[1].top < trap[1].bottom) trapout[i++] = trap[1];

	return i;
}


// 将多边形转化为trap, 并返回trap的个数, [0, n * 2] 个
// 需要提供工作内存，大小为 sizeof(ipixel_point_fixed_t) * n
int ipixel_traps_from_polygon(ipixel_trapezoid_t* trap,
	const ipixel_point_fixed_t* PTS, int n, int clockwise, void* workmem)
{
	ipixel_point_fixed_t* pts = (ipixel_point_fixed_t*)workmem;
	const ipixel_point_fixed_t* a, * b, * c;
	int index = 0;
	if (n < 3) return 0;
	memcpy(pts, PTS, sizeof(ipixel_point_fixed_t) * n);
	for (; n >= 3; ) {
		int pos = -1, k;
		if (n == 3) a = &pts[0], b = &pts[1], c = &pts[2], pos = 2;
		else {
			long s;
			for (pos = n - 1; pos >= 0; pos--) {
				a = &pts[(pos > 0) ? pos - 1 : n - 1];
				b = &pts[pos];
				c = &pts[(pos == n - 1) ? 0 : pos + 1];
				// S = (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x)
				s = ((long)(a.x - c.x)) * (b.y - c.y) -
					((long)(a.y - c.y)) * (b.x - c.x);
				if ((clockwise != 0 && s > 0) || (clockwise == 0 && s < 0)) {
					break;
				}
			}
			if (pos < 0) break;
		}
		k = ipixel_traps_from_triangle(trap + index, a, b, c);
		index += k;
		for (k = pos; k < n - 1; k++) pts[k] = pts[k + 1];
		n--;
	}
	return index;
}


// 简易多边形转化为trap，不需要额外提供内存，在栈上分配了，参数相同
int ipixel_traps_from_polygon_ex(ipixel_trapezoid_t* trap,
	 ipixel_point_fixed_t* pts, int n, int clockwise)
{
	byte _buffer[2048];
	byte* buffer = _buffer;
	int size = sizeof(ipixel_point_fixed_t) * n;
	int count;
	if (size > 2048) {
		buffer = (byte*)malloc(size);
		if (buffer == null) return 0;
	}
	count = ipixel_traps_from_polygon(trap, pts, n, clockwise, buffer);
	if (buffer != _buffer) {
		free(buffer);
	}
	return count;
}



// 像素读取

int ipixel_span_fetch(const IBITMAP* image, int offset, int line,
	int width, uint* card, ipixel_transform_t* t,
	iBitmapFetchProc proc, byte* mask, ref IRECT clip)
{
	ipixel_vector_t vec, step;
	IRECT rect;
	vec.vector[0] = cfixed_from_int(offset) + cfixed_const_half;
	vec.vector[1] = cfixed_from_int(line) + cfixed_const_half;
	vec.vector[2] = cfixed_const_1;
	if (t != null) {
		if (ipixel_transform_point(t, &vec) != 0) {
			return -1;
		}
		step.vector[0] = t.matrix[0][0];
		step.vector[1] = t.matrix[1][0];
		step.vector[2] = t.matrix[2][0];
	}
	else {
		step.vector[0] = cfixed_const_1;
		step.vector[1] = 0;
		step.vector[2] = 0;
	}
	if (IPIXEL_IS_ZERO(step.vector[2])) {
		step.vector[2] = 0;
		if (IPIXEL_IS_ONE(vec.vector[2])) vec.vector[2] = cfixed_const_1;
		if (vec.vector[2] != cfixed_const_1) {
			cfixed w = vec.vector[2];
			vec.vector[0] = cfixed_div(vec.vector[0], w);
			vec.vector[1] = cfixed_div(vec.vector[1], w);
			vec.vector[2] = cfixed_const_1;
			step.vector[0] = cfixed_div(step.vector[0], w);
			step.vector[1] = cfixed_div(step.vector[1], w);
			step.vector[2] = cfixed_div(step.vector[2], w);
		}
	}
	if (clip == null) {
		clip = &rect;
		rect.left = 0;
		rect.top = 0;
		rect.right = (int)image.w;
		rect.bottom = (int)image.h;
	}

	proc(image, card, width, &vec.vector[0], &step.vector[0], mask, clip);
	return 0;
}


iBitmapFetchProc ipixel_span_get_proc(ref IBITMAP image,
	 ipixel_transform_t* t)
{
	int filter = 0;
	int mode = 0;
	int overflow;
	int fmt;

	if (t == null) {
		mode = 1;
	}
	else if (ipixel_transform_is_identity(t)) {
		mode = 1;
	}
	else if (ipixel_transform_is_int_translate(t)) {
		mode = 1;
	}
	else if (ipixel_transform_is_scale_translate(t)) {
		mode = 2;
	}

	if (ibitmap_imode_const(image, filter) == IPIXEL_FILTER_BILINEAR)
		filter = 1;

	fmt = ibitmap_pixfmt_guess(image);
	overflow = (int)ibitmap_overflow_get(image);

	if (filter == 0) {
		if (ipixel_transform_is_translate(t)) {
			mode = 2 + overflow * 4;
			return ibitmap_scanline_get_proc(fmt, mode, 0);
		}
	}

	if (mode == 0) {
		if (filter == 0) {
			return ibitmap_scanline_get_proc(fmt,
				IBITMAP_FETCH_GENERAL_NEAREST, 0);
		}
		else {
			return ibitmap_scanline_get_proc(fmt,
				IBITMAP_FETCH_GENERAL_BILINEAR, 0);
		}
	}

	mode = 2 + overflow * 4 + (mode - 1) * 2 + filter;

	assert(mode >= 2 && mode <= 17);

	return ibitmap_scanline_get_proc(fmt, mode, 0);
}



//=====================================================================
// 位图透视/仿射变换
//=====================================================================

// 低层次光栅化位图
int ibitmap_raster_low(IBITMAP* dst, ipixel_point_fixed_t* pts,
	in IBITMAP src, ref IRECT rect, uint color, int flags,
	 ref IRECT clip, void* workmem)
{
	int ntraps, width, height, startx, starty, endx, i, j, sm, sn;
	const iColorIndex* dindex = (const iColorIndex*)dst.extra;
	byte* workptr = (byte*)workmem;
	ipixel_trapezoid_t traps[4];
	ipixel_point_fixed_t spts[4];
	ipixel_transform_t matrix;
	iBitmapFetchProc fetch;
	iSpanDrawProc draw;
	iStoreProc store;
	IRECT bound_trap;
	IRECT bound_dst;
	IRECT bound_src;
	IRECT bound_sub;
	IBITMAP* alpha;
	uint* card;
	byte* cover;
	int perspective;
	int subpixel;
	int direct;

	// 没有传递目标裁剪矩形时给一个默认的
	if (clip == null) {
		clip = &bound_dst;
		bound_dst.left = 0;
		bound_dst.top = 0;
		bound_dst.right = (int)dst.w;
		bound_dst.bottom = (int)dst.h;
	}

	// 没有传递工作内存的话，返回需要的大小
	if (workmem == null) {
		int size = 0;
		bound_trap.left = 0x7fff;
		bound_trap.right = -0x7fff;
		bound_trap.top = 0x7fff;
		bound_trap.bottom = -0x7fff;
		for (i = 0; i < 4; i++) {
			int x1 = cfixed_to_int(pts[i].x) - 2;
			int x2 = cfixed_to_int(pts[i].x) + 2;
			int y1 = cfixed_to_int(pts[i].y) - 2;
			int y2 = cfixed_to_int(pts[i].y) + 2;
			if (x1 < bound_trap.left) bound_trap.left = x1;
			if (x2 > bound_trap.right) bound_trap.right = x2;
			if (y1 < bound_trap.top) bound_trap.top = y1;
			if (y2 > bound_trap.bottom) bound_trap.bottom = y2;
		}
		ipixel_rect_intersection(&bound_trap, clip);
		width = bound_trap.right - bound_trap.left;
		size = sizeof(IBITMAP) + sizeof(void*) + (((width)+3) & ~3);
		size += sizeof(uint) * width;
		return size;
	}

	// 没有传递源裁剪矩形的话，给一个默认的
	if (rect == null) {
		rect = &bound_src;
		bound_src.left = 0;
		bound_src.top = 0;
		bound_src.right = (int)src.w;
		bound_src.bottom = (int)src.h;
	}

	// 不是直接绘制
	direct = 0;

	// 判断是否没有变换
	if (pts[0].x == pts[1].x && pts[2].x == pts[3].x &&
		pts[0].y == pts[3].y && pts[1].y == pts[2].y) {
		if (cfixed_frac(pts[0].x) == 0 && cfixed_frac(pts[0].y) == 0 &&
			cfixed_frac(pts[2].x) == 0 && cfixed_frac(pts[2].y) == 0) {
			int neww = cfixed_to_int(pts[2].x - pts[0].x);
			int newh = cfixed_to_int(pts[2].y - pts[0].y);
			if (neww == rect.right - rect.left &&
				newh == rect.bottom - rect.top) {
				direct = 1;
			}
		}
	}

	// 如果没有变换则直接绘制
	if (direct == 1) {
		int dx = cfixed_to_int(pts[0].x);
		int dy = cfixed_to_int(pts[0].y);
		int sx = rect.left;
		int sy = rect.top;
		int sw = rect.right - rect.left;
		int sh = rect.bottom - rect.top;
		int sfmt = ibitmap_pixfmt_guess(src);
		int dfmt = ibitmap_pixfmt_guess(dst);
		const iColorIndex* sindex = (const iColorIndex*)src.extra;
		const iColorIndex* dindex = (const iColorIndex*)dst.extra;
		int operate = 0;
		if (ibitmap_clipex(dst, &dx, &dy, src, &sx, &sy,
			&sw, &sh, clip, 0) != 0) {
			return -1;
		}
		if (flags & IBITMAP_RASTER_FLAG_ADD) {
			operate = IPIXEL_BLEND_OP_ADD;
		}
		else if (flags & IBITMAP_RASTER_FLAG_COPY) {
			operate = IPIXEL_BLEND_OP_COPY;
		}
		else {
			operate = IPIXEL_BLEND_OP_BLEND;
		}
		ipixel_blend(dfmt, dst.line[dy], (int)dst.pitch, dx, sfmt,
			src.line[sy], (int)src.pitch, sx, sw, sh, color,
			operate, 0, dindex, sindex, workmem);
		return 0;
	}

	// 计算是否有凹角，或者四边形边缘相交
	for (i = 0, sm = 0, sn = 0; i < 4; i++) {
		const ipixel_point_fixed_t* p1, * p2, * p3;
		long s;
		p1 = &pts[(i == 0) ? 3 : i - 1];
		p2 = &pts[i];
		p3 = &pts[(i == 3) ? 0 : i + 1];
		s = ((long)(p1.x - p3.x)) * (p2.y - p3.y) -
			((long)(p1.y - p3.y)) * (p2.x - p3.x);
		if (s >= 0) sm++;
		else sn++;
	}

	// 凹多边形
	if (sm != 4 && sn != 4) return -2;

	// 将四边形转化为若干个方便扫描的梯形
	i = ipixel_traps_from_triangle(traps, &pts[0], &pts[1], &pts[2]);
	j = ipixel_traps_from_triangle(traps + i, &pts[2], &pts[3], &pts[0]);
	ntraps = i + j;

	if (ntraps == 0) return -3;

	// 计算包围范围
	ipixel_trapezoid_bound(traps, ntraps, &bound_trap);
	ipixel_rect_intersection(&bound_trap, clip);

	startx = bound_trap.left;
	starty = bound_trap.top;
	width = bound_trap.right - bound_trap.left;
	height = bound_trap.bottom - bound_trap.top;
	endx = bound_trap.right;

	if (width <= 0 || height <= 0) return -4;
	if (clip.right <= clip.left || clip.bottom <= clip.top) return -5;
	if (rect.right <= rect.left || rect.bottom <= rect.top) return -6;

	// 初始化源坐标点
	spts[0].x = cfixed_from_int(rect.left);
	spts[0].y = cfixed_from_int(rect.top);
	spts[1].x = cfixed_from_int(rect.left);
	spts[1].y = cfixed_from_int(rect.bottom);
	spts[2].x = cfixed_from_int(rect.right);
	spts[2].y = cfixed_from_int(rect.bottom);
	spts[3].x = cfixed_from_int(rect.right);
	spts[3].y = cfixed_from_int(rect.top);

	// 判断是否需要透视投影
	if ((flags & IBITMAP_RASTER_FLAG_AFFINE) == 0) perspective = 1;
	else perspective = 0;

	// 求解变换矩阵
	if (perspective) {
		if (ipixel_transform_init_perspective(&matrix, pts, spts) != 0)
			return -7;
	}
	else {
		if (ipixel_transform_init_affine(&matrix, pts, spts) != 0)
			return -8;
	}

	// 初始化工作内存
	card = (uint*)workptr;
	workptr += sizeof(uint) * width;

	alpha = (IBITMAP*)workptr;
	workptr += sizeof(IBITMAP);
	alpha.line = (void**)workptr;
	workptr += sizeof(void*);
	alpha.pixel = (void*)workptr;
	alpha.line[0] = alpha.pixel;
	alpha.w = width;
	alpha.h = 1;
	alpha.pitch = (width + 3) & ~3;
	alpha.bpp = 8;
	alpha.code = 0;
	alpha.mode = 0;
	alpha.extra = null;
	ibitmap_pixfmt_set(alpha, IPIX_FMT_A8);

	// 检测反走样模式
	subpixel = ibitmap_imode(dst, subpixel);
	ibitmap_imode(alpha, subpixel) = subpixel;

	if (subpixel != IPIXEL_SUBPIXEL_1)
		memset(alpha.pixel, 0, alpha.pitch);

	// Alpha Map的裁剪矩形
	bound_sub.left = 0;
	bound_sub.top = 0;
	bound_sub.right = width;
	bound_sub.bottom = 1;

	// 得到扫描线取得函数
	fetch = ipixel_span_get_proc(src, &matrix);

	// 得到绘制函数
	draw = ipixel_get_span_proc(ibitmap_pixfmt_guess(dst),
		(flags & IBITMAP_RASTER_FLAG_ADD) ? 1 : 0, 0);

	// 取得存储的函数
	store = ipixel_get_store(ibitmap_pixfmt_guess(dst), 0);

	// 判断合法的颜色索引器
	if (dindex == null) dindex = _ipixel_dst_index;

	// 主要绘制循环
	for (j = 0; j < height; j++) {
		int line = starty + j;
		int xl, xr, xw;

		// 取得该扫描的X轴左右对应坐标
		if (ipixel_trapezoid_line_bound(traps, ntraps, line, &xl, &xr) != 0)
			continue;

		if (xl >= xr || xr <= startx || xl >= startx + width)
			continue;

		if (xl < startx) xl = startx;
		if (xr > endx) xr = endx;

		if (xl >= xr || line < clip.top || line >= clip.bottom)
			continue;

		xw = xr - xl;

		// 判断是否需要AA
		if (subpixel != IPIXEL_SUBPIXEL_1) {
			ipixel_raster_traps(alpha, traps, ntraps, -startx, -line,
				&bound_sub);
			cover = ((byte*)alpha.pixel) + xl - startx;
		}
		else {
			cover = null;
		}

		// 取得本行图像
		ipixel_span_fetch(src, xl, line, xw, card, &matrix, fetch,
			null, rect);

		// 颜色加成
		if (color != 0xffffffff)
			ipixel_card_multi(card, xw, color);

		// 绘制图像
		if (flags & IBITMAP_RASTER_FLAG_COPY) {
			store(dst.line[line], card, xl, xw, dindex);
		}
		else {	// 绘制线段
			if (cover == null) {
				draw(dst.line[line], xl, xw, card, null, dindex);
			}
			else {
				int fastmode = 0;
#define ISPAN_FAST	8
#if 1
				if (xw >= (ISPAN_FAST * 2 + 8)) {
					fastmode = (cover[ISPAN_FAST - 1] == 255 &&
						cover[xw - ISPAN_FAST] == 255);
				}
#endif
				if (fastmode) {
					int fastsize = xw - ISPAN_FAST * 2;
					byte* cc = cover;
					uint* ss = card;
					draw(dst.line[line], xl, ISPAN_FAST, ss, cc, dindex);
					xl += ISPAN_FAST;
					ss += ISPAN_FAST;
					cc += ISPAN_FAST;
					draw(dst.line[line], xl, fastsize, ss, null, dindex);
					xl += fastsize;
					ss += fastsize;
					cc += fastsize;
					draw(dst.line[line], xl, ISPAN_FAST, ss, cc, dindex);
				}
				else {
					draw(dst.line[line], xl, xw, card, cover, dindex);
				}
			}
		}

		// 清空 Alpha Map
		if (cover) memset(cover, 0, xw);
	}

	return 0;
}


// 低层次光栅化：不需要工作内存，在栈上分配了
int ibitmap_raster_base(IBITMAP* dst, ipixel_point_fixed_t* pts,
	in IBITMAP src, ref IRECT rect, uint color, int flags,
	 ref IRECT clip)
{
	byte _buffer[IBITMAP_STACK_BUFFER];
	byte* buffer = _buffer;
	int size, retval;

	size = ibitmap_raster_low(dst, pts, src, rect, color, flags, clip, null);

	if (size > IBITMAP_STACK_BUFFER) {
		buffer = (byte*)malloc(size);
		if (buffer == null);
		return -1;
	}

	retval = ibitmap_raster_low(dst, pts, src, rect, color, flags,
		clip, buffer);

	if (buffer != _buffer) {
		free(buffer);
	}

	return retval;
}


// 低层次光栅化：浮点参数
int ibitmap_raster_float(IBITMAP* dst, ipixel_point_t* pts,
	in IBITMAP src, ref IRECT rect, uint color, int flags,
	const ref IRECT clip)
{
	ipixel_point_fixed_t p[4];
	p[0].x = cfixed_from_double(pts[0].x);
	p[0].y = cfixed_from_double(pts[0].y);
	p[1].x = cfixed_from_double(pts[1].x);
	p[1].y = cfixed_from_double(pts[1].y);
	p[2].x = cfixed_from_double(pts[2].x);
	p[2].y = cfixed_from_double(pts[2].y);
	p[3].x = cfixed_from_double(pts[3].x);
	p[3].y = cfixed_from_double(pts[3].y);
	return ibitmap_raster_base(dst, p, src, rect, color, flags, clip);
}


//=====================================================================
// 高层次光栅化
//=====================================================================

// 旋转/缩放绘制
int ibitmap_raster_draw(IBITMAP* dst, double x, double y, in IBITMAP src,
	 ref IRECT rect, double offset_x, double offset_y, double scale_x,
	double scale_y, double theta, uint color, ref IRECT clip)
{
	ipixel_point_t pts[4];
	IRECT srcrect;
	int w, h, i;
	int flags = 0;

	if (rect == null) {
		rect = &srcrect;
		srcrect.left = 0;
		srcrect.top = 0;
		srcrect.right = (int)src.w;
		srcrect.bottom = (int)src.h;
	}

	w = rect.right - rect.left;
	h = rect.bottom - rect.top;

	pts[0].x = 0 - offset_x;
	pts[0].y = 0 - offset_y;
	pts[1].x = pts[0].x;
	pts[1].y = pts[0].y + h;
	pts[2].x = pts[0].x + w;
	pts[2].y = pts[1].y;
	pts[3].x = pts[2].x;
	pts[3].y = pts[0].y;

	for (i = 0; i < 4; i++) {
		if (scale_x != 1.0) pts[i].x *= scale_x;
		if (scale_y != 1.0) pts[i].y *= scale_y;
	}

	if (theta != 0.0) {
		double cosz, sinz;
		cosz = cos(-theta * (3.1415926535 / 180.0));
		sinz = sin(-theta * (3.1415926535 / 180.0));
		for (i = 0; i < 4; i++) {
			double x = pts[i].x;
			double y = pts[i].y;
			pts[i].x = x * cosz - y * sinz;
			pts[i].y = x * sinz + y * cosz;
		}
	}

	for (i = 0; i < 4; i++) {
		pts[i].x += x;
		pts[i].y += y;
	}

	flags = IBITMAP_RASTER_FLAG_AFFINE;

	return ibitmap_raster_float(dst, pts, src, rect, color, flags, clip);
}


// 三维旋转/缩放绘制
int ibitmap_raster_draw_3d(IBITMAP* dst, double x, double y, double z,
	in IBITMAP src, ref IRECT rect, double offset_x, double offset_y,
	double scale_x, double scale_y, double angle_x, double angle_y,
	double angle_z, uint color, ref IRECT clip)
{
	ipixel_point_t pts[4];
	double points[5][3];
	double cosx, sinx;
	double cosy, siny;
	double cosz, sinz;
	double farz, dist;
	double NX, NY, NZ;
	IRECT rectsrc;
	int width;
	int height;
	int cc, i;

#if 1
	if (angle_x == 0.0 && angle_y == 0.0) {
		return ibitmap_raster_draw(dst, x, y, src, rect, offset_x, offset_y,
			scale_x, scale_y, angle_z, color, clip);
	}
#endif

	if (rect == null) {
		rect = &rectsrc;
		rectsrc.left = 0;
		rectsrc.top = 0;
		rectsrc.right = (int)src.w;
		rectsrc.bottom = (int)src.h;
	}

	width = (rect.right - rect.left);
	height = (rect.bottom - rect.top);

	// 初始化
	points[0][0] = -offset_x;
	points[0][1] = -offset_y;
	points[0][2] = 0.0;
	points[1][0] = points[0][0];
	points[1][1] = points[0][1] + height;
	points[1][2] = 0.0;
	points[2][0] = points[1][0] + width;
	points[2][1] = points[1][1];
	points[2][2] = 0.0;
	points[3][0] = points[2][0];
	points[3][1] = points[0][1];
	points[3][2] = 0.0;
	points[4][0] = 0.0;
	points[4][1] = 0.0;
	points[4][2] = 1.0;

	// 缩放
	for (i = 0; i < 4; i++) {
		points[i][0] *= scale_x;
		points[i][1] *= scale_y;
	}

	// 计算旋转
	cosx = cos(angle_x * (3.1415926535 / 180.0));
	sinx = sin(angle_x * (3.1415926535 / 180.0));
	cosy = cos(angle_y * (3.1415926535 / 180.0));
	siny = sin(angle_y * (3.1415926535 / 180.0));
	cosz = cos(-angle_z * (3.1415926535 / 180.0));
	sinz = sin(-angle_z * (3.1415926535 / 180.0));

	// 旋转变换
	for (i = 0; i < 5; i++) {
		double mx = points[i][0];
		double my = points[i][1];
		double mz = points[i][2];
		// 围绕X轴旋转
		double nx = mx;
		double ny = my * cosx - mz * sinx;
		double nz = my * sinx + mz * cosx;
		// 围绕Y轴转动
		mx = nx * cosy + nz * siny;
		my = ny;
		mz = -nx * siny + nz * cosy;
		// 围绕Z轴转动
		points[i][0] = mx * cosz - my * sinz;
		points[i][1] = mx * sinz + my * cosz;
		points[i][2] = mz;
	}

	// 计算Z轴坐标
	if (width * scale_x > height * scale_y)
		farz = (z + 2.0) * (width * scale_x);
	else
		farz = (z + 2.0) * (height * scale_y);

	// 透视变换
	for (i = 0; i < 4; i++) {
		double px = points[i][0];
		double py = points[i][1];
		double pz = points[i][2] + farz;
		// 透视投影
		pts[i].x = x + px * farz / pz;
		pts[i].y = y + py * farz / pz;
	}

	// 计算法向量
	NX = points[4][0];
	NY = points[4][1];
	NZ = points[4][2];
	dist = sqrt(NX * NX + NY * NY + NZ * NZ);
	dist = (dist == 0.0) ? 0.0 : 1.0 / dist;

	// 法向量归一化
	NX = NX * dist;
	NY = NY * dist;
	NZ = NZ * dist;

	dist = ((NZ >= 0.0) ? NZ : (-NZ)) * 191.0;

	cc = (int)dist + 64;

	if (color == 0xffffffff) {
		color = 0xff000000 | (cc << 16) | (cc << 8) | cc;
	}
	else {
		int r, g, b, a;
		IRGBA_FROM_A8R8G8B8(color, r, g, b, a);
		r *= cc;
		g *= cc;
		b *= cc;
		r = (r + (r >> 8)) >> 8;
		g = (g + (g >> 8)) >> 8;
		b = (b + (b >> 8)) >> 8;
		color = IRGBA_TO_A8R8G8B8(r, g, b, a);
	}

	return ibitmap_raster_float(dst, pts, src, rect, color, 0, clip);
}



//=====================================================================
// 色彩梯度部分
//=====================================================================
void ipixel_gradient_walker_init(ipixel_gradient_walker_t* walker,
	const ipixel_gradient_t* gradient)
{
	walker.nstops = gradient.nstops;
	walker.stops = (ipixel_gradient_stop_t*)gradient.stops;
	walker.left_x = 0;
	walker.right_x = 0x10000;
	walker.stepper = 0;
	walker.left_ag = 0;
	walker.left_rb = 0;
	walker.right_ag = 0;
	walker.right_rb = 0;
	walker.overflow = gradient.overflow;
	walker.transparent = gradient.transparent;
	walker.need_reset = 1;
}

void ipixel_gradient_walker_reset(ipixel_gradient_walker_t* walker,
	long pos_fixed_48_16)
{
	long x, left_x, right_x;
	uint left_c, right_c;
	ipixel_gradient_stop_t* stops = walker.stops;
	int n, count = walker.nstops;
	long pos = pos_fixed_48_16;

	switch (walker.overflow)
	{
	case IBOM_WRAP:
		x = ((int)pos) & 0xffff;
		for (n = 0; n < count; n++)
			if ((int)x < stops[n].x) break;
		if (n == 0) {
			left_x = stops[count - 1].x - 0x10000;
			left_c = stops[count - 1].color;
		}
		else {
			left_x = stops[n - 1].x;
			left_c = stops[n - 1].color;
		}
		if (n == count) {
			right_x = stops[0].x + 0x10000;
			right_c = stops[0].color;
		}
		else {
			right_x = stops[n].x;
			right_c = stops[n].color;
		}
		left_x += (pos - x);
		right_x += (pos - x);
		break;

	case IBOM_REPEAT:
		for (n = 0; n < count; n++)
			if (pos < stops[n].x) break;
		if (n == 0) {
			left_x = ((long)0x80000000) << 32;
			left_c = stops[0].color;
		}
		else {
			left_x = stops[n - 1].x;
			left_c = stops[n - 1].color;
		}
		if (n == count) {
			right_x = ((long)0x7fffffff) << 32;
			right_c = stops[n - 1].color;
		}
		else {
			right_x = stops[n].x;
			right_c = stops[n].color;
		}
		break;

	case IBOM_MIRROR:
		x = (int)(pos & 0xffff);
		if ((int)pos & 0x10000)
			x = 0x10000 - x;
		for (n = 0; n < count; n++)
			if (x < stops[n].x) break;
		if (n == 0) {
			left_x = -stops[0].x;
			left_c = stops[0].color;
		}
		else {
			left_x = stops[n - 1].x;
			left_c = stops[n - 1].color;
		}
		if (n == count) {
			right_x = 0x20000 - stops[n - 1].x;
			right_c = stops[n - 1].color;
		}
		else {
			right_x = stops[n].x;
			right_c = stops[n].color;
		}
		if ((int)pos & 0x10000) {
			uint tmp_c;
			long tmp_x;
			tmp_x = 0x10000 - right_x;
			right_x = 0x10000 - left_x;
			left_x = tmp_x;
			tmp_c = right_c;
			right_c = left_c;
			left_c = tmp_c;
			x = 0x10000 - x;
		}
		left_x += (pos - x);
		right_x += (pos - x);
		break;

	default:
		for (n = 0; n < count; n++)
			if (pos < stops[n].x) break;
		if (n == 0) {
			left_x = ((long)0x80000000) << 32;
			right_x = stops[0].x;
			left_c = right_c = walker.transparent;
		}
		else if (n == count) {
			left_x = stops[count - 1].x;
			right_x = ((long)0x7fffffff) << 32;
			left_c = right_c = walker.transparent;
		}
		else {
			left_x = stops[n - 1].x;
			right_x = stops[n].x;
			left_c = stops[n - 1].color;
			right_c = stops[n].color;
		}
		break;
	}

	walker.left_x = left_x;
	walker.right_x = right_x;
	walker.left_c = left_c;
	walker.right_c = right_c;
	walker.left_ag = (left_c & 0xff00ff00) >> 8;
	walker.left_rb = (left_c & 0x00ff00ff);
	walker.right_ag = (right_c & 0xff00ff00) >> 8;
	walker.right_rb = (right_c & 0x00ff00ff);

	if (walker.left_x == walker.right_x ||
		(walker.left_ag == walker.right_ag &&
			walker.left_rb == walker.right_rb)) {
		walker.stepper = 0;
	}
	else {
		long width = (long)(right_x - left_x);
		if (width > 0) {
			walker.stepper = (int)(((1 << 24) + width / 2) / width);
		}
		else {
			walker.stepper = 0;
		}
	}

	walker.need_reset = 0;
}


#define IPIXEL_GRADIENT_NEED_RESET(w, x) \
	((w).need_reset || (x) < (w).left_x || (x) >= (w).right_x)

uint ipixel_gradient_walker_pixel(ipixel_gradient_walker_t* walker,
	long pos_fixed_48_16)
{
	int dist, idist;
	uint t1, t2, a, color;
	long x = pos_fixed_48_16;

	if (IPIXEL_GRADIENT_NEED_RESET(walker, x))
		ipixel_gradient_walker_reset(walker, x);

	if (walker.left_c == walker.right_c)
		return walker.left_c;

	dist = ((int)(x - walker.left_x) * walker.stepper) >> 16;
	idist = 256 - dist;

	t1 = walker.left_rb * idist + walker.right_rb * dist;
	t1 = (t1 >> 8) & 0xff00ff;

	t2 = walker.left_ag * idist + walker.right_ag * dist;
	t2 &= 0xff00ff00;

	color = t2 & 0xff000000;
	a = color >> 24;
	a = a;

	return color | (t1 & 0xff00ff) | (t2 & 0xff00);
}



//=====================================================================
// 色彩源实现
//=====================================================================

// 通用梯度初始化
public static void ipixel_gradient_init_common(ipixel_gradient_t* gradient,
	const ipixel_gradient_stop_t* stops, int nstops)
{
	gradient.stops = (ipixel_gradient_stop_t*)stops;
	gradient.nstops = nstops;
	gradient.overflow = IBOM_REPEAT;
	gradient.transparent = 0;
}

// 计算点乘
public static inline long ipixel_fixed_dot(long x1, long y1, long z1,
	long x2, long y2, long z2)
{
	return x1 * x2 + y1 * y2 + z1 * z2;
}

// 浮点数点乘
public static inline double ipixel_float_dot(double x1, double y1, double z1,
	double x2, double y2, double z2)
{
	return x1 * x2 + y1 * y2 + z1 * z2;
}

// 初始化线性梯度
public static void ipixel_gradient_linear_init(ipixel_gradient_linear_t* gradient,
	const ipixel_point_fixed_t* p1, ipixel_point_fixed_t* p2,
	const ipixel_gradient_stop_t* stops, int nstops)
{
	ipixel_gradient_init_common(&gradient.gradient, stops, nstops);
	gradient.p1.x = p1.x;
	gradient.p1.y = p1.y;
	gradient.p2.x = p2.x;
	gradient.p2.y = p2.y;
}

// 初始化放射梯度
public static void ipixel_gradient_radial_init(ipixel_gradient_radial_t* gradient,
	const ipixel_point_fixed_t* inner, ipixel_point_fixed_t* outer,
	cfixed inner_radius, cfixed outer_radius,
	const ipixel_gradient_stop_t* stops, int nstops)
{
	ipixel_gradient_init_common(&gradient.gradient, stops, nstops);
	gradient.x1 = inner.x;
	gradient.y1 = inner.y;
	gradient.r1 = inner_radius;
	gradient.x2 = outer.x;
	gradient.y2 = outer.y;
	gradient.r2 = outer_radius;
	gradient.xd = gradient.x2 - gradient.x1;
	gradient.yd = gradient.y2 - gradient.y1;
	gradient.rd = gradient.r2 - gradient.r1;
	gradient.a = (double)ipixel_fixed_dot(gradient.xd, gradient.yd,
		-gradient.rd, gradient.xd, gradient.yd, gradient.rd);
	if (gradient.a != 0)
		gradient.inva = 1.0 * cfixed_const_1 / gradient.a;
	else
		gradient.inva = 0.0;
	gradient.mindr = -1.0 * cfixed_const_1 * gradient.r1;
}

#define IPX_MOD(a, b) ((a) < 0 ? ((b) - ((-(a) - 1) % (b))) - 1 : (a) % (b))

// 初始化锥型梯度
public static void ipixel_gradient_conical_init(ipixel_gradient_conical_t* gradient,
	const ipixel_point_fixed_t* center, cfixed angle,
	const ipixel_gradient_stop_t* stops, int nstops)
{
	ipixel_gradient_init_common(&gradient.gradient, stops, nstops);

	angle = IPX_MOD(angle, cfixed_from_int(360));
	gradient.center = *center;
	gradient.angle = cfixed_to_double(angle) * 3.141592653589793 / 180.0;
}

// 更新 fetch
public static inline void ipixel_source_update(ipixel_source_t* source)
{
	if (source.type == IPIXEL_SOURCE_BITMAP) {
		source.fetch = ipixel_span_get_proc(source.source.bitmap,
			source.transform);
	}
}

// 初始化位图源
void ipixel_source_init_bitmap(ipixel_source_t* source, in IBITMAP  bmp)
{
	source.type = IPIXEL_SOURCE_BITMAP;
	source.source.bitmap = bmp;
	source.overflow = (int)ibitmap_imode(bmp, overflow);
	source.transparent = (uint)bmp.mask;
	source.srect.left = 0;
	source.srect.top = 0;
	source.srect.right = (int)bmp.w;
	source.srect.bottom = (int)bmp.h;
	source.transform = null;
	source.fetch = null;
	ipixel_source_update(source);
}

// 初始化固定颜色源
void ipixel_source_init_solid(ipixel_source_t* source, uint color)
{
	source.type = IPIXEL_SOURCE_SOLID;
	source.source.solid.color = color;
	source.fetch = null;
}

// 初始化线性梯度源
void ipixel_source_init_gradient_linear(ipixel_source_t* source,
	const ipixel_point_fixed_t* p1, ipixel_point_fixed_t* p2,
	const ipixel_gradient_stop_t* stops, int nstops)
{
	source.type = IPIXEL_SOURCE_LINEAR;
	source.transform = null;
	ipixel_gradient_linear_init(&source.source.linear, p1, p2,
		stops, nstops);
	source.transparent = 0;
	source.overflow = IBOM_REPEAT;
	source.fetch = null;
}

// 初始化辐射源
void ipixel_source_init_gradient_radial(ipixel_source_t* source,
	const ipixel_point_fixed_t* inner, ipixel_point_fixed_t* outer,
	cfixed inner_radius, cfixed outer_radius,
	const ipixel_gradient_stop_t* stops, int nstops)
{
	source.type = IPIXEL_SOURCE_RADIAL;
	source.transform = null;
	ipixel_gradient_radial_init(&source.source.radial, inner, outer,
		inner_radius, outer_radius, stops, nstops);
	source.transparent = 0;
	source.overflow = IBOM_REPEAT;
	source.fetch = null;
}

// 初始化锥体源
void ipixel_source_init_gradient_conical(ipixel_source_t* source,
	const ipixel_point_fixed_t* center, cfixed angle,
	const ipixel_gradient_stop_t* stops, int nstops)
{
	source.type = IPIXEL_SOURCE_CONICAL;
	source.transform = null;
	ipixel_gradient_conical_init(&source.source.conical, center, angle,
		stops, nstops);
	source.transparent = 0;
	source.overflow = IBOM_REPEAT;
	source.fetch = null;
}


// 设置变换矩阵：矩阵指针变化但内容不变时也要调用
void ipixel_source_set_transform(ipixel_source_t* source,
	const ipixel_transform_t* t)
{
	source.transform = (ipixel_transform_t*)t;
	source.fetch = null;
	ipixel_source_update(source);
}

// 设置越界方式
void ipixel_source_set_overflow(ipixel_source_t* source,
	 IBOM overflow, uint transparent)
{
	source.overflow = (int)overflow;
	source.transparent = transparent;

	switch (source.type)
	{
	case IPIXEL_SOURCE_BITMAP:
		ibitmap_imode(source.source.bitmap, overflow) = (int)overflow;
		source.source.bitmap.mask = (uint)transparent;
		break;

	case IPIXEL_SOURCE_SOLID:
		break;

	case IPIXEL_SOURCE_LINEAR:
		source.source.linear.gradient.overflow = (int)overflow;
		source.source.linear.gradient.transparent = transparent;
		break;

	case IPIXEL_SOURCE_RADIAL:
		source.source.radial.gradient.overflow = (int)overflow;
		source.source.radial.gradient.transparent = transparent;
		break;

	case IPIXEL_SOURCE_CONICAL:
		source.source.conical.gradient.overflow = (int)overflow;
		source.source.conical.gradient.transparent = transparent;
		break;
	}
	ipixel_source_update(source);
}

// 设置过滤器
void ipixel_source_set_filter(ipixel_source_t* source,
	 IPIXELFILTER filter)
{
	if (source.type == IPIXEL_SOURCE_BITMAP) {
		ibitmap_imode(source.source.bitmap, filter) = (int)filter;
		ipixel_source_update(source);
	}
}

// 设置裁剪矩形
void ipixel_source_set_bound(ipixel_source_t* source,
	const ref IRECT bound)
{
	IBITMAP* src;
	IRECT size;
	if (source.type != IPIXEL_SOURCE_BITMAP)
		return;
	src = source.source.bitmap;
	size.left = 0;
	size.top = 0;
	size.right = (int)src.w;
	size.bottom = (int)src.h;
	if (bound == null) {
		source.srect = size;
	}
	else {
		source.srect = *bound;
		ipixel_rect_intersection(&source.srect, &size);
	}
}


// 取得固定颜色扫描线
public static int ipixel_source_fetch_solid(const ipixel_source_t* source,
	int offset, int line, int width, uint* card, byte* mask)
{
	uint color = source.source.solid.color;
	_ipixel_fill_32(card, 0, width, color);
	return 0;
}

// 取得位图的扫描线
public static int ipixel_source_fetch_bitmap(const ipixel_source_t* source,
	int offset, int line, int width, uint* card, byte* mask)
{
	IBITMAP* src;
	int retval;
	src = source.source.bitmap;
	if (source.fetch == null)
		return -1000;
	retval = ipixel_span_fetch(src, offset, line, width, card,
		source.transform, source.fetch, mask, &source.srect);
	return retval;
}


// 取得线性梯度的扫描线
public static int ipixel_source_fetch_linear(const ipixel_source_t* source,
	int offset, int line, int width, uint* card, byte* mask)
{
	const ipixel_gradient_linear_t* linear = &source.source.linear;
	const ipixel_gradient_t* gradient = &linear.gradient;
	ipixel_gradient_walker_t walker;
	ipixel_vector_t v, unit;
	long l, dx, dy;
	uint* endup = card + width;
	const public static byte mask0[2] = { 255, 255 };
	int incmask = 1;

	if (gradient.stops == null || gradient.nstops < 1)
		return -1;

	ipixel_gradient_walker_init(&walker, gradient);

	if (mask == null) mask = mask0, incmask = 0;

	v.vector[0] = cfixed_from_int(offset) + cfixed_const_half;
	v.vector[1] = cfixed_from_int(line) + cfixed_const_half;
	v.vector[2] = cfixed_const_1;

	if (source.transform) {
		if (ipixel_transform_point(source.transform, &v) != 0)
			return -2;
		unit.vector[0] = source.transform.matrix[0][0];
		unit.vector[1] = source.transform.matrix[1][0];
		unit.vector[2] = source.transform.matrix[2][0];
	}
	else {
		unit.vector[0] = cfixed_const_1;
		unit.vector[1] = 0;
		unit.vector[2] = 0;
	}

	dx = linear.p2.x - linear.p1.x;
	dy = linear.p2.y - linear.p1.y;

	l = dx * dx + dy * dy;

	if (l == 0 || unit.vector[2] == 0) {
		// 插值，无透视
		long t, next_inc;
		double inc;
		if (l == 0 || v.vector[2] == 0) {
			t = 0;
			inc = 0;
		}
		else {
			double invden, v2;
			invden = cfixed_const_1 * (double)cfixed_const_1 /
				(l * (double)v.vector[2]);
			v2 = v.vector[2] * (1.0 / cfixed_const_1);
			t = (long)(((dx * v.vector[0] + dy * v.vector[1]) -
				(dx * linear.p1.x + dy * linear.p1.y) * v2) * invden);
			inc = (dx * unit.vector[0] + dy * unit.vector[1]) * invden;
		}

		next_inc = 0;

		if (((long)inc * width) == 0) {
			uint color = ipixel_gradient_walker_pixel(&walker, t);
			while (card < endup) *card++ = color;
		}
		else {
			int i;
			for (i = 0; card < endup; mask += incmask, card++) {
				if (mask[0]) {
					card[0] = ipixel_gradient_walker_pixel(&walker,
						t + next_inc);
				}
				i++;
				next_inc = (long)(inc * i);
			}
		}
	}
	else {
		// 透视
		double t;
		for (t = 0; card < endup; mask += incmask, card++) {
			if (mask[0]) {
				if (v.vector[2] != 0) {
					double invden, v2;
					invden = cfixed_const_1 * (double)cfixed_const_1 /
						(l * (double)v.vector[2]);
					v2 = v.vector[2] * (1.0 / cfixed_const_1);
					t = ((dx * v.vector[0] + dy * v.vector[1]) -
						(dx * linear.p1.x + dy * linear.p1.y) * v2) *
						invden;
				}
				card[0] = ipixel_gradient_walker_pixel(&walker, (long)t);
			}
			v.vector[0] += unit.vector[0];
			v.vector[1] += unit.vector[1];
			v.vector[2] += unit.vector[2];
		}
	}

	return 0;
}

public static inline uint ipixel_radial_compute(double a, double b, double c,
	double inva, double dr, double mindr, ipixel_gradient_walker_t* walker)
{
	double det;
	if (a == 0.0 && b != 0.0) {
		return ipixel_gradient_walker_pixel(walker,
			(long)(cfixed_const_half * c / b));
	}
	det = ipixel_float_dot(b, a, 0, b, -c, 0);
	if (det >= 0) {
		double sqrtdet, t0, t1;
		sqrtdet = sqrt(det);
		t0 = (b + sqrtdet) * inva;
		t1 = (b - sqrtdet) * inva;
		if (walker.overflow == IBOM_TRANSPARENT) {
			if (0 <= t0 && t0 <= cfixed_const_1)
				return ipixel_gradient_walker_pixel(walker, (long)t0);
			else if (0 <= t1 && t1 <= cfixed_const_1)
				return ipixel_gradient_walker_pixel(walker, (long)t1);
		}
		else {
			if (t0 * dr > mindr)
				return ipixel_gradient_walker_pixel(walker, (long)t0);
			else if (t1 * dr > mindr)
				return ipixel_gradient_walker_pixel(walker, (long)t1);
		}
	}

	return walker.transparent;
}

// 取得线性梯度的扫描线
public static int ipixel_source_fetch_radial(const ipixel_source_t* source,
	int offset, int line, int width, uint* card, byte* mask)
{
	const ipixel_gradient_radial_t* radial = &source.source.radial;
	const ipixel_gradient_t* gradient = &radial.gradient;
	ipixel_gradient_walker_t walker;
	uint* endup = card + width;
	ipixel_vector_t v, unit;
	const public static byte mask0[2] = { 255, 255 };
	int incmask = 1;

	if (gradient.stops == null || gradient.nstops < 1)
		return -1;

	if (mask == null) mask = mask0, incmask = 0;

	v.vector[0] = cfixed_from_int(offset) + cfixed_const_half;
	v.vector[1] = cfixed_from_int(line) + cfixed_const_half;
	v.vector[2] = cfixed_const_1;

	ipixel_gradient_walker_init(&walker, gradient);

	if (source.transform) {
		if (ipixel_transform_point(source.transform, &v) != 0)
			return -2;
		unit.vector[0] = source.transform.matrix[0][0];
		unit.vector[1] = source.transform.matrix[1][0];
		unit.vector[2] = source.transform.matrix[2][0];
	}
	else {
		unit.vector[0] = cfixed_const_1;
		unit.vector[1] = 0;
		unit.vector[2] = 0;
	}

	if (unit.vector[2] == 0 && v.vector[2] == cfixed_const_1) {
		// 插值变换
		long b, db, c, dc, ddc;
		v.vector[0] -= radial.x1;
		v.vector[1] -= radial.y1;
		b = ipixel_fixed_dot(v.vector[0], v.vector[1], radial.r1,
			radial.xd, radial.yd, radial.rd);
		db = ipixel_fixed_dot(unit.vector[0], unit.vector[1], 0,
			radial.xd, radial.yd, 0);

		c = ipixel_fixed_dot(v.vector[0], v.vector[1],
			-((long)radial.r1),
			v.vector[0], v.vector[1], radial.r1);
		dc = ipixel_fixed_dot(2 * (long)v.vector[0] + unit.vector[0],
			2 * (long)v.vector[1] + unit.vector[1], 0,
			unit.vector[0], unit.vector[1], 0);
		ddc = 2 * ipixel_fixed_dot(unit.vector[0], unit.vector[1], 0,
			unit.vector[0], unit.vector[1], 0);

		for (; card < endup; mask += incmask, card++) {
			if (mask[0]) {
				card[0] = ipixel_radial_compute(
					radial.a, (double)b, (double)c, radial.inva, radial.rd,
					radial.mindr, &walker);
			}
			b += db;
			c += dc;
			dc += ddc;
		}
	}
	else {
		// 透视变换
		uint transparent = source.transparent;
		for (; card < endup; mask += incmask, card++) {
			if (mask[0]) {
				if (v.vector[2] != 0) {
					double pdx, pdy, invv2, b, c;
					invv2 = 1.0 * cfixed_const_1 / v.vector[2];
					pdx = v.vector[0] * invv2 - radial.x1;
					pdy = v.vector[1] * invv2 - radial.y1;
					b = ipixel_float_dot(pdx, pdy, radial.r1,
						radial.xd, radial.yd, radial.rd);
					c = ipixel_float_dot(pdx, pdy, -radial.r1,
						pdx, pdy, radial.r1);
					card[0] = ipixel_radial_compute(
						radial.a, b, c,
						radial.inva, radial.rd, radial.mindr,
						&walker);
				}
				else {
					card[0] = transparent;
				}
			}
			v.vector[0] += unit.vector[0];
			v.vector[1] += unit.vector[1];
			v.vector[2] += unit.vector[2];
		}
	}

	return 0;
}

public static inline double ipixel_conical_coordinate(double x, double y, double a)
{
	double t;
	t = atan2(y, x) + a;
	while (t < 0) t += 3.141592653589793 * 2.0;
	while (t >= 2.0 * 3.141592653589793) t -= 3.141592653589793 * 2.0;
	return 1 - t * (1.0 / (2 * 3.141592653589793));
}

// 取得线性梯度的扫描线
public static int ipixel_source_fetch_conical(const ipixel_source_t* source,
	int offset, int line, int width, uint* card, byte* mask)
{
	const ipixel_gradient_conical_t* conical = &source.source.conical;
	const ipixel_gradient_t* gradient = &conical.gradient;
	const public static byte mask0[2] = { 255, 255 };
	ipixel_gradient_walker_t walker;
	uint* endup = card + width;
	double cx = 1.0, cy = 0.0, cz = 0.0;
	double rx = offset + 0.5;
	double ry = line + 0.5;
	double rz = 1.0;
	int affine = 1;
	int incmask = 1;

	if (gradient.stops == null || gradient.nstops < 1)
		return -1;

	if (mask == null) mask = mask0, incmask = 0;

	ipixel_gradient_walker_init(&walker, gradient);

	if (source.transform) {
		ipixel_vector_t v;
		v.vector[0] = cfixed_from_int(offset) + cfixed_const_half;
		v.vector[1] = cfixed_from_int(line) + cfixed_const_half;
		v.vector[2] = cfixed_const_1;
		if (ipixel_transform_point(source.transform, &v) != 0)
			return -2;
		cx = source.transform.matrix[0][0] / 65536.0;
		cy = source.transform.matrix[1][0] / 65536.0;
		cz = source.transform.matrix[2][0] / 65536.0;
		rx = v.vector[0] / 65536.0;
		ry = v.vector[1] / 65536.0;
		rz = v.vector[2] / 65536.0;
		affine = source.transform.matrix[2][0] == 0 &&
			v.vector[2] == cfixed_const_1;
	}

	if (affine) {
		// 仿射变换
		rx -= conical.center.x / 65536.0;
		ry -= conical.center.y / 65536.0;
		for (; card < endup; mask += incmask, card++) {
			if (mask[0]) {
				double t = ipixel_conical_coordinate(rx, ry, conical.angle);
				card[0] = ipixel_gradient_walker_pixel(&walker,
					(long)(t * 65536.0));
			}
			rx += cx;
			ry += cy;
		}
	}
	else {
		// 投影变换
		double centerx = conical.center.x / 65536.0;
		double centery = conical.center.y / 65536.0;
		for (; card < endup; mask += incmask, card++) {
			double x, y;
			if (mask[0]) {
				double t;
				if (rz != 0) {
					x = rx / rz;
					y = ry / rz;
				}
				else {
					x = y = 0.0;
				}
				x -= centerx;
				y -= centery;
				t = ipixel_conical_coordinate(x, y, conical.angle);
				card[0] = ipixel_gradient_walker_pixel(&walker,
					(long)(t * 65536.0));
			}
			rx += cx;
			ry += cy;
			rz += cz;
		}
	}

	return 0;
}



// 取得扫描线
int ipixel_source_fetch(const ipixel_source_t* source, int offset, int line,
	int width, uint* card, byte* mask)
{
	int retval = -100;
	switch (source.type)
	{
	case IPIXEL_SOURCE_BITMAP:
		retval = ipixel_source_fetch_bitmap(source, offset, line, width,
			card, mask);
		break;

	case IPIXEL_SOURCE_SOLID:
		retval = ipixel_source_fetch_solid(source, offset, line, width,
			card, mask);
		break;

	case IPIXEL_SOURCE_LINEAR:
		retval = ipixel_source_fetch_linear(source, offset, line, width,
			card, mask);
		break;

	case IPIXEL_SOURCE_RADIAL:
		retval = ipixel_source_fetch_radial(source, offset, line, width,
			card, mask);
		break;

	case IPIXEL_SOURCE_CONICAL:
		retval = ipixel_source_fetch_conical(source, offset, line, width,
			card, mask);
		break;
	}

	return retval;
}


