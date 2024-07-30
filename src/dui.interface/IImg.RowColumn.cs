using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d2;

public partial interface IImg
{

    /// <summary> Represents a list of rows of an image. </summary>
    public interface IImageRows : IEnumerable<IImageRow>
    {
        /// <summary>
        /// Gets the amount of rows in this list.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a specific <see cref="IImageRow"/>.
        /// </summary>
        /// <param name="column">The ´row to get.</param>
        /// <returns>The requested <see cref="IImageRow"/>.</returns>
        IImageRow this[int column] { get; }
    }

    /// <summary>
    /// Represents a list of columns of an image.
    /// </summary>
    public interface IImageColumns : IEnumerable<IImageColumn>
    {
        /// <summary>
        /// Gets the amount of columns in this list.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a specific <see cref="IImageColumn"/>.
        /// </summary>
        /// <param name="column">The column to get.</param>
        /// <returns>The requested <see cref="IImageColumn"/>.</returns>
        IImageColumn this[int column] { get; }
    }

    /// <summary>
    /// Represents a single row of an image.
    /// </summary>
    public interface IImageRow : IEnumerable<IColor>
    {
        /// <summary>
        /// Gets the length of the row.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the size in bytes of this row.
        /// </summary>
        int SizeInBytes { get; }

        /// <summary>
        /// Gets the <see cref="IColor"/> at the specified location.
        /// </summary>
        /// <param name="x">The location to get the color from.</param>
        /// <returns>The <see cref="IColor"/> at the specified location.</returns>
        IColor this[int x] { get; }

        /// <summary>
        /// Copies the contents of this <see cref="IImageRow"/> into a destination <see cref="Span{T}"/> instance.
        /// </summary>
        /// <param name="destination">The destination <see cref="Span{T}"/> instance.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="destination"/> is shorter than the source <see cref="IImageRow"/> instance.
        /// </exception>
        void CopyTo (in Span<byte> destination);

        /// <summary>
        /// Allocates a new array and copies this <see cref="IImageRow"/> into it.
        /// </summary>
        /// <returns>The new array containing the data of this <see cref="IImageRow"/>.</returns>
        byte[] ToArray ();
    }

    /// <summary>Represents a single column of an image.</summary>
    public interface IImageColumn : IEnumerable<IColor>
    {
        /// <summary> Gets the length of the column. </summary>
        int Length { get; }

        /// <summary> Gets the size in bytes of this column. </summary>
        int SizeInBytes { get; }

        /// <summary>
        /// Gets the <see cref="IColor"/> at the specified location.
        /// </summary>
        /// <param name="y">The location to get the color from.</param>
        /// <returns>The <see cref="IColor"/> at the specified location.</returns>
        IColor this[int y] { get; }

        /// <summary>
        /// Copies the contents of this <see cref="IImageColumn"/> into a destination <see cref="Span{T}"/> instance.
        /// </summary>
        /// <param name="destination">The destination <see cref="Span{T}"/> instance.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="destination"/> is shorter than the source <see cref="IImageColumn"/> instance.
        /// </exception>
        void CopyTo (in Span<byte> destination);

        /// <summary>
        /// Allocates a new array and copies this <see cref="IImageColumn"/> into it.
        /// </summary>
        /// <returns>The new array containing the data of this <see cref="IImageColumn"/>.</returns>
        byte[] ToArray ();
    }

}
