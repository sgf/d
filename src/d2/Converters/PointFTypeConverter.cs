using System;
using System.ComponentModel;
using System.Globalization;

namespace d2;

public class PointFTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		=> sourceType == typeof(string);

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		=> destinationType == typeof(string);

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (PtF.TryParse(value?.ToString(), out var p))
			return p;

		throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(PtF)));
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (!(value is PtF p))
			throw new NotSupportedException();
		return $"{p.X.ToString(CultureInfo.InvariantCulture)}, {p.Y.ToString(CultureInfo.InvariantCulture)}";
	}
}
