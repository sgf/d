using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace d2
{
    public class raylib
    {

        /// <summary>Draw pixel within an image</summary>
        public static void ImageDrawPixel(Image* dst, int posX, int posY, Color color);

        /// <summary>Draw pixel within an image (Vector version)</summary>
        public static void ImageDrawPixelV(Image* dst, Vector2 position, Color color);

        /// <summary>Draw line within an image</summary>
        public static void ImageDrawLine(
            Image* dst,
            int startPosX,
            int startPosY,
            int endPosX,
            int endPosY,
            Color color
        );

        /// <summary>Draw line within an image (Vector version)</summary>
        public static void ImageDrawLineV(Image* dst, Vector2 start, Vector2 end, Color color);

        /// <summary>Draw circle within an image</summary>
        public static void ImageDrawCircle(Image* dst, int centerX, int centerY, int radius, Color color);

        /// <summary>Draw circle within an image (Vector version)</summary>
        public static void ImageDrawCircleV(Image* dst, Vector2 center, int radius, Color color);

        /// <summary>Draw circle outline within an image</summary>
        public static void ImageDrawCircleLines(Image* dst, int centerX, int centerY, int radius, Color color);

        /// <summary>Draw circle outline within an image (Vector version)</summary>
        public static void ImageDrawCircleLinesV(Image* dst, Vector2 center, int radius, Color color);

        /// <summary>Draw rectangle within an image</summary>
        public static void ImageDrawRectangle(
            Image* dst,
            int posX,
            int posY,
            int width,
            int height,
            Color color
        );

        /// <summary>Draw rectangle within an image (Vector version)</summary>
        public static void ImageDrawRectangleV(Image* dst, Vector2 position, Vector2 size, Color color);

        /// <summary>Draw rectangle within an image</summary>
        public static void ImageDrawRectangleRec(Image* dst, Rectangle rec, Color color);

        /// <summary>Draw rectangle lines within an image</summary>
        public static void ImageDrawRectangleLines(Image* dst, Rectangle rec, int thick, Color color);

        /// <summary>Draw a source image within a destination image (tint applied to source)</summary>
        public static void ImageDraw(Image* dst, Image src, Rectangle srcRec, Rectangle dstRec, Color tint);

        /// <summary>Draw text (using default font) within an image (destination)</summary>
        public static void ImageDrawText(Image* dst, sbyte* text, int x, int y, int fontSize, Color color);

        /// <summary>Draw text (custom sprite font) within an image (destination)</summary>
        public static void ImageDrawTextEx(
            Image* dst,
            Font font,
            sbyte* text,
            Vector2 position,
            float fontSize,
            float spacing,
            Color tint
        );



        // Texture loading functions
        // NOTE: These functions require GPU access

        /// <summary>Load texture from file into GPU memory (VRAM)</summary>
        public static Texture2D LoadTexture(sbyte* fileName);

        /// <summary>Load texture from image data</summary>
        public static Texture2D LoadTextureFromImage(Image image);

        /// <summary>Load cubemap from image, multiple image cubemap layouts supported</summary>
        public static Texture2D LoadTextureCubemap(Image image, CubemapLayout layout);

        /// <summary>Load texture for rendering (framebuffer)</summary>
        public static RenderTexture2D LoadRenderTexture(int width, int height);

        /// <summary>Check if a texture is ready</summary>
        public static CBool IsTextureReady(Texture2D texture);

        /// <summary>Unload texture from GPU memory (VRAM)</summary>
        public static void UnloadTexture(Texture2D texture);

        /// <summary>Check if a render texture is ready</summary>
        public static CBool IsRenderTextureReady(RenderTexture2D target);

        /// <summary>Unload render texture from GPU memory (VRAM)</summary>
        public static void UnloadRenderTexture(RenderTexture2D target);

        /// <summary>Update GPU texture with new data</summary>
        public static void UpdateTexture(Texture2D texture, void* pixels);

        /// <summary>Update GPU texture rectangle with new data</summary>
        public static void UpdateTextureRec(Texture2D texture, Rectangle rec, void* pixels);


        // Texture configuration functions

        /// <summary>Generate GPU mipmaps for a texture</summary>
        public static void GenTextureMipmaps(Texture2D* texture);

        /// <summary>Set texture scaling filter mode</summary>
        public static void SetTextureFilter(Texture2D texture, TextureFilter filter);

        /// <summary>Set texture wrapping mode</summary>
        public static void SetTextureWrap(Texture2D texture, TextureWrap wrap);


        // Texture drawing functions

        /// <summary>Draw a Texture2D</summary>
        public static void DrawTexture(Texture2D texture, int posX, int posY, Color tint);

        /// <summary>Draw a Texture2D with position defined as Vector2</summary>
        public static void DrawTextureV(Texture2D texture, Vector2 position, Color tint);

        /// <summary>Draw a Texture2D with extended parameters</summary>
        public static void DrawTextureEx(
            Texture2D texture,
            Vector2 position,
            float rotation,
            float scale,
            Color tint
        );

        /// <summary>Draw a part of a texture defined by a rectangle</summary>
        public static void DrawTextureRec(Texture2D texture, Rectangle source, Vector2 position, Color tint);

        /// <summary>Draw a part of a texture defined by a rectangle with 'pro' parameters</summary>
        public static void DrawTexturePro(
            Texture2D texture,
            Rectangle source,
            Rectangle dest,
            Vector2 origin,
            float rotation,
            Color tint
        );

        /// <summary>Draws a texture (or part of it) that stretches or shrinks nicely</summary>
        public static void DrawTextureNPatch(
            Texture2D texture,
            NPatchInfo nPatchInfo,
            Rectangle dest,
            Vector2 origin,
            float rotation,
            Color tint
        );


        // Color/pixel related functions

        /// <summary>Get hexadecimal value for a Color</summary>
        public static int ColorToInt(Color color);

        /// <summary>Get color normalized as float [0..1]</summary>
        public static Vector4 ColorNormalize(Color color);

        /// <summary>Get color from normalized values [0..1]</summary>
        public static Color ColorFromNormalized(Vector4 normalized);

        /// <summary>Get HSV values for a Color, hue [0..360], saturation/value [0..1]</summary>
        public static Vector3 ColorToHSV(Color color);

        /// <summary>Get a Color from HSV values, hue [0..360], saturation/value [0..1]</summary>
        public static Color ColorFromHSV(float hue, float saturation, float value);

        /// <summary>Get color multiplied with another color</summary>
        public static Color ColorTint(Color color, Color tint);

        /// <summary>Get color with brightness correction, brightness factor goes from -1.0f to 1.0f</summary>
        public static Color ColorBrightness(Color color, float factor);

        /// <summary>Get color with contrast correction, contrast values between -1.0f and 1.0f</summary>
        public static Color ColorContrast(Color color, float contrast);

        /// <summary>Get color with alpha applied, alpha goes from 0.0f to 1.0f</summary>
        public static Color ColorAlpha(Color color, float alpha);

        /// <summary>Get src alpha-blended into dst color with tint</summary>
        public static Color ColorAlphaBlend(Color dst, Color src, Color tint);

        /// <summary>Get Color structure from hexadecimal value</summary>
        public static Color GetColor(uint hexValue);

        /// <summary>Get Color from a source pixel pointer of certain format</summary>
        public static Color GetPixelColor(void* srcPtr, PixelFormat format);

        /// <summary>Set color formatted into destination pixel pointer</summary>
        public static void SetPixelColor(void* dstPtr, Color color, PixelFormat format);

        /// <summary>Get pixel data size in bytes for certain format</summary>
        public static int GetPixelDataSize(int width, int height, PixelFormat format);


        //------------------------------------------------------------------------------------
        // Font Loading and Text Drawing Functions (Module: text)
        //------------------------------------------------------------------------------------

        // Font loading/unloading functions

        /// <summary>Get the default Font</summary>
        public static Font GetFontDefault();

        /// <summary>Load font from file into GPU memory (VRAM)</summary>
        public static Font LoadFont(sbyte* fileName);

        /// <summary>
        /// Load font from file with extended parameters, use NULL for fontChars and 0 for glyphCount to load
        /// the default character set
        /// </summary>
        public static Font LoadFontEx(sbyte* fileName, int fontSize, int* codepoints, int codepointCount);

        /// <summary>Load font from Image (XNA style)</summary>
        public static Font LoadFontFromImage(Image image, Color key, int firstChar);

        /// <summary>Load font from memory buffer, fileType refers to extension: i.e. "ttf"</summary>
        public static Font LoadFontFromMemory(
            sbyte* fileType,
            byte* fileData,
            int dataSize,
            int fontSize,
            int* codepoints,
            int codepointCount
        );

        /// <summary>Check if a font is ready</summary>
        public static CBool IsFontReady(Font font);

        /// <summary>Load font data for further use</summary>
        public static GlyphInfo* LoadFontData(
            byte* fileData,
            int dataSize,
            int fontSize,
            int* fontChars,
            int glyphCount,
            FontType type
        );

        /// <summary>Generate image font atlas using chars info</summary>
        public static Image GenImageFontAtlas(
            GlyphInfo* chars,
            Rectangle** recs,
            int glyphCount,
            int fontSize,
            int padding,
            int packMethod
        );

        /// <summary>Unload font chars info data (RAM)</summary>
        public static void UnloadFontData(GlyphInfo* chars, int glyphCount);

        /// <summary>Unload Font from GPU memory (VRAM)</summary>
        public static void UnloadFont(Font font);

        /// <summary>Export font as code file, returns true on success</summary>
        public static CBool ExportFontAsCode(Font font, sbyte* fileName);


        // Text drawing functions

        /// <summary>Shows current FPS</summary>
        public static void DrawFPS(int posX, int posY);

        /// <summary>Draw text (using default font)</summary>
        public static void DrawText(sbyte* text, int posX, int posY, int fontSize, Color color);

        /// <summary>Draw text using font and additional parameters</summary>
        public static void DrawTextEx(
            Font font,
            sbyte* text,
            Vector2 position,
            float fontSize,
            float spacing,
            Color tint
        );

        /// <summary>Draw text using Font and pro parameters (rotation)</summary>
        public static void DrawTextPro(
            Font font,
            sbyte* text,
            Vector2 position,
            Vector2 origin,
            float rotation,
            float fontSize,
            float spacing,
            Color tint
        );

        /// <summary>Draw one character (codepoint)</summary>
        public static void DrawTextCodepoint(
            Font font,
            int codepoint,
            Vector2 position,
            float fontSize,
            Color tint
        );

        /// <summary>Draw multiple characters (codepoint)</summary>
        public static void DrawTextCodepoints(
            Font font,
            int* codepoints,
            int count,
            Vector2 position,
            float fontSize,
            float spacing,
            Color tint
        );

        // Text font info functions

        /// <summary>Set vertical line spacing when drawing with line-breaks</summary>
        public static void SetTextLineSpacing(int spacing);

        /// <summary>Measure string width for default font</summary>
        public static int MeasureText(sbyte* text, int fontSize);

        /// <summary>Measure string size for Font</summary>
        public static Vector2 MeasureTextEx(Font font, sbyte* text, float fontSize, float spacing);

        /// <summary>
        /// Get glyph index position in font for a codepoint (unicode character), fallback to '?' if not found
        /// </summary>
        public static int GetGlyphIndex(Font font, int character);

        /// <summary>
        /// Get glyph font info data for a codepoint (unicode character), fallback to '?' if not found
        /// </summary>
        public static GlyphInfo GetGlyphInfo(Font font, int codepoint);

        /// <summary>
        /// Get glyph rectangle in font atlas for a codepoint (unicode character), fallback to '?' if not found
        /// </summary>
        public static Rectangle GetGlyphAtlasRec(Font font, int codepoint);


        // Text codepoints management functions (unicode characters)

        /// <summary>Load UTF-8 text encoded from codepoints array</summary>
        public static sbyte* LoadUTF8(int* codepoints, int length);

        /// <summary>Unload UTF-8 text encoded from codepoints array</summary>
        public static void UnloadUTF8(sbyte* text);

        /// <summary>Load all codepoints from a UTF-8 text string, codepoints count returned by parameter</summary>
        public static int* LoadCodepoints(sbyte* text, int* count);

        /// <summary>Unload codepoints data from memory</summary>
        public static void UnloadCodepoints(int* codepoints);

        /// <summary>Get total number of codepoints in a UTF8 encoded string</summary>
        public static int GetCodepointCount(sbyte* text);

        /// <summary>Get next codepoint in a UTF-8 encoded string, 0x3f('?') is returned on failure</summary>
        public static int GetCodepoint(sbyte* text, int* codepointSize);

        /// <summary>Get next codepoint in a UTF-8 encoded string; 0x3f('?') is returned on failure</summary>
        public static int GetCodepointNext(sbyte* text, int* codepointSize);

        /// <summary>Get previous codepoint in a UTF-8 encoded string, 0x3f('?') is returned on failure</summary>
        public static int GetCodepointPrevious(sbyte* text, int* codepointSize);

        /// <summary>Encode one codepoint into UTF-8 byte array (array length returned as parameter)</summary>
        public static sbyte* CodepointToUTF8(int codepoint, int* utf8Size);


        // Text strings management functions (no UTF-8 strings, only byte chars)
        // NOTE: Some strings allocate memory internally for returned strings, just be careful!

        // <summary>Copy one string to another, returns bytes copied</summary>
        public static int TextCopy(sbyte* dst, sbyte* src);

        /// <summary>Check if two text string are equal</summary>
        public static CBool TextIsEqual(sbyte* text1, sbyte* text2);

        /// <summary>Get text length, checks for '\0' ending</summary>
        public static uint TextLength(sbyte* text);

        /// <summary>Text formatting with variables (sprintf style)</summary>
        public static sbyte* TextFormat(sbyte* text);

        /// <summary>Get a piece of a text string</summary>
        public static sbyte* TextSubtext(sbyte* text, int position, int length);

        /// <summary>Replace text string (WARNING: memory must be freed!)</summary>
        public static sbyte* TextReplace(sbyte* text, sbyte* replace, sbyte* by);

        /// <summary>Insert text in a position (WARNING: memory must be freed!)</summary>
        public static sbyte* TextInsert(sbyte* text, sbyte* insert, int position);

        /// <summary>Join text strings with delimiter</summary>
        public static sbyte* TextJoin(sbyte** textList, int count, sbyte* delimiter);

        /// <summary>Split text into multiple strings</summary>
        public static sbyte** TextSplit(sbyte* text, char delimiter, int* count);

        /// <summary>Append text at specific position and move cursor!</summary>
        public static void TextAppend(sbyte* text, sbyte* append, int* position);

        /// <summary>Find first text occurrence within a string</summary>
        public static int TextFindIndex(sbyte* text, sbyte* find);

        /// <summary>Get upper case version of provided string</summary>
        public static sbyte* TextToUpper(sbyte* text);

        /// <summary>Get lower case version of provided string</summary>
        public static sbyte* TextToLower(sbyte* text);

        /// <summary>Get Pascal case notation version of provided string</summary>
        public static sbyte* TextToPascal(sbyte* text);

        /// <summary>Get integer value from text (negative values not supported)</summary>
        public static int TextToInteger(sbyte* text);

    }
}
