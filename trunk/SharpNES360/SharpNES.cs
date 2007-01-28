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

        public SharpNES()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            this.IsFixedTimeStep = false;
        }

        public void LoadCart(string filename)
        {
#if XBOX
            using (StorageContainer container = saveDevice.OpenContainer("SharpNES - Roms"))
            {
                // move roms from the run directory to the roms directory
                foreach (string file in Directory.GetFiles(StorageContainer.TitleLocation, "*.nes"))
                {
                    File.Copy(file, Path.Combine(container.Path, Path.GetFileName(file)), true);
                    File.Delete(file);
                }
                myEngine.LoadCart(Path.Combine(container.Path, filename));
            }

            using (StorageContainer container = saveDevice.OpenContainer("SharpNES - Saves"))
            {
                myEngine.LoadRam();
            }

#else
            myEngine.LoadCart(filename);
            myEngine.LoadRam();
#endif

            myEngine.StartCart();
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

        StorageDevice saveDevice;
        bool storageDeviceRequested;
        IAsyncResult storageDeviceRequestResult;


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the default game to exit on Xbox 360 and Windows
            if (GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
            {
#if XBOX
                using (StorageContainer container = saveDevice.OpenContainer("SharpNES - Saves"))
                {
                    myEngine.SaveRamDirectory = container.Path;
                    myEngine.StopCart(); // Writes SaveRam
                }
#else
                myEngine.StopCart(); // Writes SaveRam
#endif
                this.Exit();
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
                        LoadCart("Zelda.nes");
                    }
                }
#else
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