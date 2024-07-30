using System;
using System.ComponentModel;
using System.Globalization;

namespace d2.Converters;

public class RectTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		=> sourceType == typeof(string);

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		=> destinationType == typeof(string);

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (RtF.TryParse(value?.ToString(), out var r))
			return r;

		throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(RtF)));
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (!(value is RtF r))
			throw new NotSupportedException();
		return $"{r.X.ToString(CultureInfo.InvariantCulture)}, {r.Y.ToString(CultureInfo.InvariantCulture)}, {r.Width.ToString(CultureInfo.InvariantCulture)}, {r.Height.ToString(CultureInfo.InvariantCulture)}";
	}
}
