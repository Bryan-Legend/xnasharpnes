#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace XNASharpNES
{
    public class SharpNES : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;

        NesEngine myEngine = new NesEngine();
        Texture2D targetTexture;
        SpriteBatch spriteBatch;

        bool running;
        string currentRom;

        public SharpNES()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            this.IsFixedTimeStep = false;
        }

        public void LoadCart(string filename)
        {
            if (running)
            {
                running = false;
                myEngine.SaveRam();
            }

            myEngine.LoadCart(filename);

#if XBOX
            using (StorageContainer container = saveDevice.OpenContainer("SharpNES"))
            {
                myEngine.LoadRam();
                SaveRomName(filename, container.Path);
            }
#else
            myEngine.LoadRam();
            SaveRomName(filename, StorageContainer.TitleLocation);
#endif

            myEngine.StartCart();

            currentRom = Path.GetFileName(filename);
            running = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                // TODO: Load any ResourceManagementMode.Automatic content
            }

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            targetTexture = new Texture2D(graphics.GraphicsDevice, 256, 224, 1, ResourceUsage.None, SurfaceFormat.Bgr565, ResourceManagementMode.Manual);
        }


        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent == true)
            {
                content.Unload();
            }
        }

        void SaveRomName(string rom, string savePath)
        {
            using (StreamWriter writer = File.CreateText(Path.Combine(savePath, "LastRom.txt")))
                writer.Write(rom);
        }

        string LoadRomName(string savePath)
        {
            string filename = Path.Combine(savePath, "LastRom.txt");
            string result = String.Empty;

            if (File.Exists(filename))
            {
                using (StreamReader reader = File.OpenText(filename))
                    result = reader.ReadLine();
            }

            if (result.Length == 0 || !File.Exists(result))
            {
                string[] roms = Directory.GetFiles(StorageContainer.TitleLocation, "*.nes");
                if (roms.Length == 0)
                    throw new Exception("No roms found.");
                Array.Sort(roms);
                result = roms[0];
            }

            return result;
        }

        void Quit()
        {
#if XBOX
            using (StorageContainer container = saveDevice.OpenContainer("SharpNES"))
            {
                myEngine.SaveRamDirectory = container.Path;
                myEngine.StopCart(); // Writes SaveRam
            }
#else
            myEngine.SaveRamDirectory = StorageContainer.TitleLocation;
            myEngine.StopCart(); // Writes SaveRam
#endif
            this.Exit();
        }

        void LoadNextRom()
        {
            string[] roms = Directory.GetFiles(StorageContainer.TitleLocation, "*.nes");
            if (roms.Length == 0)
                throw new Exception("No roms found.");
            Array.Sort(roms);
            int count;
            for (count = 0; count < roms.Length; count++)
            {
                if (Path.GetFileName(roms[count]) == currentRom)
                {
                    if (count == roms.Length - 1)
                        count = -1;
                    break;
                }
            }
            LoadCart(roms[count + 1]);
        }

        void LoadPreviousRom()
        {
            string[] roms = Directory.GetFiles(StorageContainer.TitleLocation, "*.nes");
            if (roms.Length == 0)
                throw new Exception("No roms found.");
            Array.Sort(roms);
            int count;
            for (count = 0; count < roms.Length; count++)
            {
                if (Path.GetFileName(roms[count]) == currentRom)
                {
                    if (count == 0)
                        count = roms.Length;
                    break;
                }
            }
            LoadCart(roms[count - 1]);
        }

#if XBOX
        StorageDevice saveDevice;
        bool storageDeviceRequested;
        IAsyncResult storageDeviceRequestResult;
#endif

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the default game to exit on Xbox 360 and Windows
            GamePadState pad = GamePad.GetState(PlayerIndex.One);

            if (pad.Buttons.RightShoulder == ButtonState.Pressed)
            {
                if (pad.Buttons.Back == ButtonState.Pressed)
                    Quit();

                if (pad.DPad.Left == ButtonState.Pressed || pad.DPad.Up == ButtonState.Pressed)
                    LoadPreviousRom();
                if (pad.DPad.Right == ButtonState.Pressed || pad.DPad.Down == ButtonState.Pressed)
                    LoadNextRom();
            }

            if (running)
                myEngine.RunCart();
            else
            {
#if XBOX
                if (saveDevice == null)
                {
                    if (!storageDeviceRequested)
                    {
                        storageDeviceRequested = true;
                        storageDeviceRequestResult = StorageDevice.BeginShowStorageDeviceGuide(PlayerIndex.One, null, null);
                    }

                    if ((storageDeviceRequested) && (storageDeviceRequestResult.IsCompleted))
                    {
                        saveDevice = StorageDevice.EndShowStorageDeviceGuide(storageDeviceRequestResult);

                        string filename;
                        using (StorageContainer container = saveDevice.OpenContainer("SharpNES"))
                        {
                            filename = LoadRomName(container.Path);
                        }

                        LoadCart(filename);
                    }
                }
#else
                LoadCart(LoadRomName(StorageContainer.TitleLocation));
#endif
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            if (running)
            {
                targetTexture.SetData<short>(myEngine.myPPU.offscreenBuffer, 256 * 8, targetTexture.Width * targetTexture.Height, SetDataOptions.None);

                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                spriteBatch.Draw(targetTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}