using DevcadeGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WraithHunt
{
    class WHScreens
    {
        public static void drawWinner(
            GameState state,
            GraphicsDevice GraphicsDevice,
            Viewport defaultViewport,
            SpriteBatch _spriteBatch,
            SpriteFont _HUDFont,
            SpriteFont _titleFont,
            Texture2D _redButton
        )
        {
            string textWinner, textSubtitle, textContinue;
            textContinue = "Replay?";
            switch (state)
            {
                case GameState.MEDIUM_WON:
                    GraphicsDevice.Clear(Color.DarkBlue);
                    textWinner = "Medium\n  Wins!";
                    textSubtitle = "The forces of good have prevailed!\n      When are we getting paid?";
                    break;
                case GameState.WRAITH_WON:
                    GraphicsDevice.Clear(Color.DarkRed);
                    textWinner = "Demon\n Wins!";
                    textSubtitle = "The evil that haunts this building\n   has claimed another victim.\nWill another hero rise to defeat it?";
                    break;
                default:
                    textWinner = "shit is bugged!";
                    textSubtitle = "chom.";
                    break;
            }
            Vector2 textWinnerSize = _HUDFont.MeasureString(textWinner);
            Vector2 textSubtitleSize = _HUDFont.MeasureString(textSubtitle);
            Vector2 textContinueSize = _HUDFont.MeasureString(textContinue);
            //_spriteBatch.DrawString(_HUDFont, textWinner, new Vector2(10 , defaultViewport.Height/2), Color.Gold, 0, new Vector2(0,0), 0, SpriteEffects.None, 0);
            //_spriteBatch.DrawString(_HUDFont, textWinner, new Vector2(10 , 10), Color.Gold, 0, new Vector2(0,0), 0, SpriteEffects.None, 0);
            _spriteBatch.DrawString(
                _titleFont,
                textWinner,
                new Vector2(
                    defaultViewport.Width / 2 - textWinnerSize.X - (textWinnerSize.X / 2),
                    100
                    ),
                Color.Gold
            );
            _spriteBatch.DrawString(
                _HUDFont,
                textSubtitle,
                new Vector2(
                    defaultViewport.Width / 2 - textWinnerSize.X * 2 - 10,
                    250
                    ),
                Color.Gold
            );
            _spriteBatch.DrawString(
                _HUDFont,
                textContinue,
                new Vector2(
                    defaultViewport.Width / 2 - textContinueSize.X / 2,
                    350
                    ),
                Color.Gold
            );
            _spriteBatch.Draw(
                _redButton,
                new Rectangle(
                    defaultViewport.Width / 2 - 30,
                    400,
                    60,
                    60
                ),
                null,
                Color.White,
                0,
                new Vector2(0, 0),
                SpriteEffects.None,
                0
            );
        }
    }
}