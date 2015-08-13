using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Bitmap = System.Drawing.Bitmap;
using Color = Microsoft.Xna.Framework.Color;

namespace RTS_test.GUI.Components
{
    class Button : GUIComponent
    {
        private string text;
        private SpriteFont font;
        private Color textColor;
        private Color backgroundColor;
        private int borderWidth;
        private Color borderColor;
        private Texture2D generatedTexture;
        private Texture2D generatedPressedTexture;
        private Texture2D generatedHoverTexture;


        private bool pressed = false;
        private bool hover = false;
        private Color pressedBackgroundColor;
        private Random r = new Random();

        public Button(Rectangle box, String text, SpriteFont font, Color textColor, Color backgroundColor, int borderWidth, Color borderColor, Alignment alignment = Alignment.LeftTop)
            : base(box, alignment)
        {
            this.text = text;
            this.font = font;
            this.textColor = textColor;
            this.backgroundColor = backgroundColor;
            this.borderWidth = borderWidth;
            this.borderColor = borderColor;

            pressedBackgroundColor = new Color(r.Next(256), r.Next(256), r.Next(256));

            generatedTexture = generateButtonTexture(false, false);
            generatedPressedTexture = generateButtonTexture(false, true);
            generatedHoverTexture = generateButtonTexture(true, false);



            Global.inputManager.lmbEvent += onLeftClick;
        }

        private Texture2D generateButtonTexture(bool hover, bool pressed)
        {
            Bitmap bitmap = new Bitmap(Box.Width, Box.Height);

            Point leftTop = new Point(borderWidth, borderWidth);
            Point leftBottom = new Point(borderWidth, Box.Height - borderWidth - 1);
            Point rightTop = new Point(Box.Width - borderWidth - 1, borderWidth);
            Point rightBottom = new Point(Box.Width - borderWidth - 1, Box.Height - borderWidth - 1);

            Color backgroundColor = this.backgroundColor;
            if (pressed)
            {
                backgroundColor = Color.Black;
                backgroundColor.A = 192;
            }

            bitmap = drawBitmapCircle(leftTop, borderWidth, bitmap, backgroundColor, hover);
            bitmap = drawBitmapCircle(leftBottom, borderWidth, bitmap, backgroundColor, hover);
            bitmap = drawBitmapCircle(rightTop, borderWidth, bitmap, backgroundColor, hover);
            bitmap = drawBitmapCircle(rightBottom, borderWidth, bitmap, backgroundColor, hover);



            for (int x = borderWidth; x < Box.Width - borderWidth; x++)
            {
                for (int y = 0; y < Box.Height; y++)
                {
                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B));
                }
            }

            for (int x = 0; x < Box.Width; x++)
            {
                for (int y = borderWidth; y < Box.Height - borderWidth; y++)
                {
                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B));
                }
            }

            if (hover)
            {
                for (int x = borderWidth; x < Box.Width - borderWidth; x++)
                {
                    for (int y = 0; y < Box.Height; y++)
                    {
                        if (x == 0 || y == 0 || x == Box.Width - 1 || y == Box.Height - 1)
                            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(borderColor.A, borderColor.R, borderColor.G, borderColor.B));
                    }
                }

                for (int x = 0; x < Box.Width; x++)
                {
                    for (int y = borderWidth; y < Box.Height - borderWidth; y++)
                    {
                        if (x == 0 || y == 0 || x == Box.Width - 1 || y == Box.Height - 1)
                            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(borderColor.A, borderColor.R, borderColor.G, borderColor.B));
                    }
                }
            }

            return BitmapToTexture2D(EntitySystem.BlackBoard.GetEntry<SpriteBatch>("SpriteBatch").GraphicsDevice, bitmap);
        }

        private Bitmap drawBitmapCircle(Point pos, int radius, Bitmap bitmap, Color color, bool border)
        {
            for (int x = pos.X - radius; x < pos.X + radius; x++)
            {
                for (int y = pos.Y - radius; y < pos.Y + radius; y++)
                {
                    if (border)
                    {
                        double dis = Math.Pow((double)(pos.Y - y), 2) + Math.Pow((double)(pos.X - x), 2);
                        if (dis <= Math.Pow((double)radius, 2) && dis >= Math.Pow((double)radius - 1, 2))
                            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(borderColor.A, borderColor.R, borderColor.G, borderColor.B));
                        else if (dis < Math.Pow((double)radius, 2))
                            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
                    }
                    else
                    {
                        if (Math.Pow((double)(pos.Y - y), 2) + Math.Pow((double)(pos.X - x), 2) <= Math.Pow((double)radius, 2))
                            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
                    }
                }
            }
            return bitmap;
        }

        private void onLeftClick(bool isDown)
        {
            if (GlobalBox.Contains(Mouse.GetState().Position))
            {
                pressed = isDown;
                if (Global.inputManager.lmb.Released)
                {
                    // New mouse click
                    //Console.WriteLine("click!");
                }
            }
            else
                pressed = false;
        }

        public static Texture2D BitmapToTexture2D(GraphicsDevice GraphicsDevice, System.Drawing.Bitmap image)
        {
            // Buffer size is size of color array multiplied by 4 because   
            // each pixel has four color bytes  
            int bufferSize = image.Height * image.Width * 4;

            // Create new memory stream and save image to stream so   
            // we don't have to save and read file  
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(bufferSize);
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Creates a texture from IO.Stream - our memory stream  
            Texture2D texture = Texture2D.FromStream(
              GraphicsDevice, memoryStream);

            return texture;
        }

        public override void update(GameTime gameTime)
        {
            if (GlobalBox.Contains(Mouse.GetState().Position))
            {
                hover = true;
            }
            else
                hover = false;
            base.update(gameTime);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (hover && !pressed)
                spriteBatch.Draw(generatedHoverTexture, null, GlobalBox, new Rectangle(0, 0, Box.Width, Box.Height), null, 0f, null, null, SpriteEffects.None, 0);
            else if (pressed)
                spriteBatch.Draw(generatedPressedTexture, null, GlobalBox, new Rectangle(0, 0, Box.Width, Box.Height), null, 0f, null, null, SpriteEffects.None, 0);
            else
                spriteBatch.Draw(generatedTexture, null, GlobalBox, new Rectangle(0, 0, Box.Width, Box.Height), null, 0f, null, null, SpriteEffects.None, 0);


            Vector2 textSize = font.MeasureString(text);
            Vector2 drawPos = GlobalPosition.ToVector2();
            drawPos = drawPos + new Vector2((Box.Width - textSize.X) / 2, (Box.Height - textSize.Y) / 2);
            spriteBatch.DrawString(font, text, drawPos, new Microsoft.Xna.Framework.Color(textColor.R, textColor.G, textColor.B, textColor.A));
            base.draw(spriteBatch);
        }
    }
}
