using System;
using System.Collections;
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
        private static readonly Color BackgroundColor = Color.White;
        private static readonly Color TitleColor = Color.White;
        private static readonly Color MenuItemColor = Color.SlateGray;
        private static readonly Color SelectedItemColor = Color.Red;

        private const int NumMenuItems = 24;

        private Vector2 TitlePosition;
        private Vector2 MenuPosition;
        private int MenuItemHeight;
        #endregion

        private GraphicsDeviceManager graphics;
        private ContentManager content;

        private Texture2D targetTexture;
        private Rectangle screenRectangle;
        private SpriteBatch spriteBatch;
        private BitmapFont menuFont;
        private Texture2D titleTexture;

        private StorageDevice saveDevice;
        private string[] roms;
        private int topMenuRom;
        private int selectedRom;

        private GamePadHelper menuPad;
        private bool waitForBackRelease = false;

        private NesEngine myEngine = new NesEngine();
        private bool running = false;

        public SharpNES()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);
            IsFixedTimeStep = false;

            menuPad = new GamePadHelper(20);

            saveDevice = StorageDevice.ShowStorageDeviceGuide();
            ArrayList files = new ArrayList();
            files.AddRange(Directory.GetFiles(StorageContainer.TitleLocation));
            int i = 0;
            while (i < files.Count)
            {
                if (files[i].ToString().EndsWith(".nes"))
                    i++;
                else
                    files.RemoveAt(i);
            }
            roms = (string[])files.ToArray(typeof(string));
            Array.Sort(roms);

            string lastRom = LoadRomName();
            selectedRom = Array.IndexOf(roms, lastRom);
            if (selectedRom < 0)
            {
                selectedRom = 0;
            }

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

            targetTexture = 
                new Texture2D
                (
                    graphics.GraphicsDevice,
                    256, 224, 1,
                    ResourceUsage.None,
                    SurfaceFormat.Bgr565,
                    ResourceManagementMode.Manual
                );

            if (loadAllContent)
            {
                titleTexture = content.Load<Texture2D>("Logo");
                LoadFonts();
            }
        }

        private void LoadFonts()
        {
            menuFont = new BitmapFont(content, "menufont.xml");
            menuFont.Reset(graphics.GraphicsDevice);

            int screenWidth = graphics.PreferredBackBufferWidth;
            int screenHeight = graphics.PreferredBackBufferHeight;

            TitlePosition = new Vector2(
                (screenWidth - titleTexture.Width) / 2,
                screenHeight / 30);

            MenuItemHeight = menuFont.Baseline * 4 / 3;
            MenuPosition = new Vector2(
                screenWidth / 8,
                TitlePosition.Y + titleTexture.Height + MenuItemHeight);
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
            GamePadState pad = GamePad.GetState(PlayerIndex.One);

            if ((pad.Buttons.Back == ButtonState.Pressed) && 
                (pad.Buttons.RightShoulder == ButtonState.Pressed))
            {
                StopCart();
                waitForBackRelease = true;
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
            menuPad.Update();

            if (waitForBackRelease)
            {
                // Since the back button is used to leave the game,
                // we need to wait here for it to be released.
                if (menuPad.BackIsPressed)
                {
                    return;
                }

                menuPad.Reset();
                waitForBackRelease = false;
            }

            if (menuPad.BackIsPressed)
            {
                Quit();
            }

            if (roms.Length > 0)
            {
                if (menuPad.StartIsPressed || menuPad.AIsPressed)
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

            spriteBatch.Draw(titleTexture, TitlePosition, Color.White);

            if (roms.Length == 0)
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

                if (roms.Length > currentMenuRom)
                    menuFont.DrawString
                    (
                        (int)MenuPosition.X,
                        (int)MenuPosition.Y + index * MenuItemHeight,
                        itemColor,
                        Path.GetFileNameWithoutExtension(roms[currentMenuRom])
                    );
            }
        }

        private void SaveRomName(string rom)
        {
            using (StorageContainer container = saveDevice.OpenContainer("SharpNES"))
            {
                using (StreamWriter writer = File.CreateText(Path.Combine(container.Path, "LastRom.txt")))
                {
                    writer.Write(rom);
                }
            }
        }

        private string LoadRomName()
        {
            string result = String.Empty;

            using (StorageContainer container = saveDevice.OpenContainer("SharpNES"))
            {
                string filename = Path.Combine(container.Path, "LastRom.txt");

                if (File.Exists(filename))
                {
                    using (StreamReader reader = File.OpenText(filename))
                        result = reader.ReadLine();
                }
            }

            return result;
        }

        private void LoadCart(string filename)
        {
            SaveRomName(filename);
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