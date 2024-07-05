// ConsoleApplication1.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>

// 位图模式
struct IMODE
{
	union {
		struct f1 {
			unsigned char reserve1;		// 保留
			unsigned char reserve2;		// 保留
			unsigned char pixfmt : 6;	// 颜色格式
			unsigned char fmtset : 1;	// 是否设置颜色格式
			unsigned char overflow : 2;	// 越界采样模式，见 IBOM定义
			unsigned char filter : 2;	// 过滤器
			unsigned char refbits : 1;	// 是否引用数据
			unsigned char subpixel : 2;	// 子像素模式
		} f;
		unsigned long mode;
	};
};

int main()
{
	IMODE id;
	auto size = sizeof(id.f);
	std::cout << size;
    //std::cout << "Hello World!\n";
}

// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门使用技巧: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件
