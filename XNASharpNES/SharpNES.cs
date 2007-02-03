using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using XNAExtras;

namespace XNASharpNES
{
    public class SharpNES : Microsoft.Xna.Framework.Game
    {
        #region Menu Constants
        private static readonly Color BackgroundColor = new Color(0xcc, 0xcc, 0xcc);
        private static readonly Color TitleColor = new Color(0x66, 0x66, 0x66);
        private static readonly Color MenuItemColor = Color.Black;
        private static readonly Color SelectedItemColor = Color.Red;

        private static readonly String Title = "XNA SharpNES";
        private static readonly String LongMenuName = "BomberMan2";

        private const int NumMenuItems = 10;

        private Vector2 TitlePosition;
        private Vector2 MenuPosition;
        private int MenuItemHeight;
        #endregion

        private GraphicsDeviceManager graphics;
        private ContentManager content;

        private Texture2D targetTexture;
        private Rectangle screenRectangle;
        private SpriteBatch spriteBatch;
        private BitmapFont titleFont;
        private BitmapFont menuFont;

        private StorageDevice saveDevice;
        private GamePadHelper menuPad = new GamePadHelper();
        private string[] roms;
        private int topMenuRom;
        private int selectedRom;

        private NesEngine myEngine = new NesEngine();
        private bool running = false;

        public SharpNES()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);
            IsFixedTimeStep = false;

            saveDevice = StorageDevice.ShowStorageDeviceGuide();
            roms = Directory.GetFiles(StorageContainer.TitleLocation, "*.nes");
            selectedRom = (roms.Length > 0) ? 0 : -1;
            topMenuRom = 0;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            screenRectangle = new Rectangle(
                0, 0,
                graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight); 

            targetTexture = new Texture2D(
                graphics.GraphicsDevice, 
                256, 224, 1, 
                ResourceUsage.None, 
                SurfaceFormat.Bgr565, 
                ResourceManagementMode.Manual);

            if (loadAllContent)
            {
                LoadFonts();
            }
        }

        private void LoadFonts()
        {
            titleFont = new BitmapFont(content, "titlefont.xml");
            titleFont.Reset(graphics.GraphicsDevice);

            menuFont = new BitmapFont(content, "menufont.xml");
            menuFont.Reset(graphics.GraphicsDevice);

            int screenWidth = graphics.PreferredBackBufferWidth;
            int screenHeight = graphics.PreferredBackBufferHeight;

            int titleWidth = titleFont.MeasureString(Title);
            int titleHeight = titleFont.Baseline;
            TitlePosition = new Vector2(
                (screenWidth - titleWidth) / 2, 
                screenHeight / 10);

            int menuWidth = menuFont.MeasureString(LongMenuName);
            MenuItemHeight = menuFont.Baseline * 4 / 3;
            MenuPosition = new Vector2(
                (screenWidth - menuWidth) / 2, 
                TitlePosition.Y + (titleHeight * 3 / 2));
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent == true)
            {
                content.Unload();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            menuPad.Update();

            if (running)
            {
                UpdateGame();
            }
            else
            {
                UpdateMenu();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (running)
            {
                DrawGame();
            }
            else
            {
                DrawMenu();
            }

            base.Draw(gameTime);
        }

        private void UpdateGame()
        {
            if (menuPad.RightShoulderIsPressed && menuPad.BackWasPressed)
            {
                StopCart();
                return;
            }
            myEngine.RunCart();
        }

        private void DrawGame()
        {
            targetTexture.SetData<short>(
                myEngine.myPPU.offscreenBuffer, 
                256 * 8, 
                targetTexture.Width * targetTexture.Height, 
                SetDataOptions.None);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(targetTexture, screenRectangle, Color.White);
            spriteBatch.End();
        }

        private void UpdateMenu()
        {
            if (menuPad.BackWasPressed)
            {
                Quit();
            }

            if (selectedRom >= 0)
            {
                if (menuPad.StartWasPressed || menuPad.AWasPressed)
                {
                    LoadCart(roms[selectedRom]);
                }
                else if (menuPad.UpWasPressed || menuPad.LeftWasPressed)
                {
                    selectedRom = Math.Max(selectedRom - 1, 0);

                    if (selectedRom < topMenuRom)
                    {
                        topMenuRom--;
                    }
                }
                else if (menuPad.DownWasPressed || menuPad.RightWasPressed)
                {
                    selectedRom = Math.Min(selectedRom + 1, roms.Length - 1);

                    if (selectedRom >= topMenuRom + NumMenuItems)
                    {
                        topMenuRom++;
                    }
                }
            }
        }

        private void DrawMenu()
        {
            graphics.GraphicsDevice.Clear(BackgroundColor);
            spriteBatch.Begin(SpriteBlendMode.None);

            titleFont.DrawString(TitlePosition, TitleColor, Title);

            if (selectedRom < 0)
            {
                menuFont.DrawString(MenuPosition, SelectedItemColor, "No ROMs Available.");
            }
            else
            {
                DrawRomNames();
            }

            spriteBatch.End();
        }

        private void DrawRomNames()
        {
            for (int index = 0; index < NumMenuItems; index++)
            {
                int currentMenuRom = topMenuRom + index;
                Color itemColor = (currentMenuRom == selectedRom) ? SelectedItemColor : MenuItemColor;

                menuFont.DrawString(
                    (int)MenuPosition.X,
                    (int)MenuPosition.Y + index * MenuItemHeight, 
                    itemColor,
                    Path.GetFileNameWithoutExtension(roms.Length <= currentMenuRom ? "..." : roms[currentMenuRom]));
            }
        }

        private void LoadCart(string filename)
        {
            myEngine.LoadCart(filename);
            myEngine.LoadRam();
            myEngine.StartCart();
            running = true;
        }

        private void StopCart()
        {
            using (StorageContainer container = saveDevice.OpenContainer("SharpNES"))
            {
                myEngine.SaveRamDirectory = container.Path;
                myEngine.StopCart(); // Writes SaveRam
            }
            running = false;
        }

        private void Quit()
        {
            StopCart();
            this.Exit();
        }
    }
}