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

        private bool pressed = false;
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

            Bitmap bitmap = new Bitmap(Box.Width, Box.Height);
            for (int x = 0; x < Box.Width; x++)
            {
                for (int y = 0; y < Box.Height; y++)
                {
                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B));
                    if (x <= borderWidth || x >= Box.Width - borderWidth || y <= borderWidth || y >= Box.Height - borderWidth)
                        bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(borderColor.A, borderColor.R, borderColor.G, borderColor.B));
                }
            }
            generatedTexture = new Texture2D(EntitySystem.BlackBoard.GetEntry<SpriteBatch>("SpriteBatch").GraphicsDevice, Box.Width, Box.Height);
            generatedTexture.SetData<byte>(Button.GetBytes(bitmap));

            bitmap = new Bitmap(Box.Width, Box.Height);
            for (int x = 0; x < Box.Width; x++)
            {
                for (int y = 0; y < Box.Height; y++)
                {
                    if (x <= borderWidth || x >= Box.Width - borderWidth || y <= borderWidth || y >= Box.Height - borderWidth)
                        bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(borderColor.A, borderColor.R, borderColor.G, borderColor.B));
                    else
                        bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(pressedBackgroundColor.A, pressedBackgroundColor.R, pressedBackgroundColor.G, pressedBackgroundColor.B));

                }
            }
            generatedPressedTexture = new Texture2D(EntitySystem.BlackBoard.GetEntry<SpriteBatch>("SpriteBatch").GraphicsDevice, Box.Width, Box.Height);
            generatedPressedTexture.SetData<byte>(Button.GetBytes(bitmap));

            Global.inputManager.lmbEvent += onLeftClick;
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

        public static byte[] GetBytes(Bitmap bitmap)
        {
            var data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            // calculate the byte size: for PixelFormat.Format32bppArgb (standard for GDI bitmaps) it's the hight * stride
            int bufferSize = data.Height * data.Stride; // stride already incorporates 4 bytes per pixel

            // create buffer
            byte[] bytes = new byte[bufferSize];

            // copy bitmap data into buffer
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // unlock the bitmap data
            bitmap.UnlockBits(data);

            return bytes;

        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (!pressed)
                spriteBatch.Draw(generatedTexture, null, GlobalBox, new Rectangle(0, 0, Box.Width, Box.Height), null, 0f, null, null, SpriteEffects.None, 0);
            else
                spriteBatch.Draw(generatedPressedTexture, null, GlobalBox, new Rectangle(0, 0, Box.Width, Box.Height), null, 0f, null, null, SpriteEffects.None, 0);

            Vector2 textSize = font.MeasureString(text);
            Vector2 drawPos = GlobalPosition.ToVector2();
            drawPos = drawPos + new Vector2((Box.Width - textSize.X) / 2, (Box.Height - textSize.Y) / 2);
            spriteBatch.DrawString(font, text, drawPos, new Microsoft.Xna.Framework.Color(textColor.R, textColor.G, textColor.B, textColor.A));
            base.draw(spriteBatch);
        }
    }
}
