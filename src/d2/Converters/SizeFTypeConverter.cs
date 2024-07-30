using System;
using System.ComponentModel;
using System.Globalization;

namespace d2;

public class SizeFTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		=> sourceType == typeof(string);

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		=> destinationType == typeof(string);

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (SzF.TryParse(value?.ToString(), out var s))
			return s;

		throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(SzF)));
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (!(value is SzF size))
			throw new NotSupportedException();
		return $"{size.Width.ToString(CultureInfo.InvariantCulture)}, {size.Height.ToString(CultureInfo.InvariantCulture)}";
	}
}
