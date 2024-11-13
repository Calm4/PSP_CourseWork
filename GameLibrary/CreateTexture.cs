using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace GameLibrary
{
    public class CreateTexture
    {
        public static int LoadTexture(string texturePath)
        {

            if (!File.Exists("Sprites/" + texturePath))
            {
                throw new FileNotFoundException("File not found at 'Sprites/" + texturePath + "'");
            }

            int textureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            Bitmap bmp = new Bitmap("Sprites/" + texturePath);
            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0,0,bmp.Width,bmp.Height), 
                ImageLockMode.ReadOnly, 
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            bmp.UnlockBits(bmpData);

            //устанавливает параметры обертывания текстуры по оси S(горизонтальной оси)
            GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapS,(int)TextureWrapMode.Clamp);
            //устанавливает параметры обертывания текстуры по оси T(вертикальной оси)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            // Обработка того, как будет изменяться текстура при уменьшении ее размера 
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            // Обработка того, как будет изменяться тестура при увеличении ее размера 
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return textureID;
        }
    }
}
