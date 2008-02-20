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
using Microsoft.Xna.Framework.GamerServices;
//using XNAExtras;

namespace XNASharpNES
{
    public class SharpNES : Microsoft.Xna.Framework.Game
    {
        #region Menu Constants
        private static readonly Color BackgroundColor = Color.White;
        private static readonly Color TitleColor = Color.White;
        private static readonly Color MenuItemColor = Color.Black;
        private static readonly Color SelectedItemColor = Color.DarkGreen;

        private const int NumMenuItems = 24;

        private Vector2 MenuPosition;
        private int MenuItemHeight;
        #endregion

        private SpriteFont MenuSpriteFont;
        private GraphicsDeviceManager graphics;
        private ContentManager content;
        private SpriteBatch spriteBatch;
        private Texture2D targetTexture;
        private Texture2D backgroundTexture;
        private Texture2D helpBackgroundTexture;
        private GamePadHelper menuPad;
        private Rectangle screenRectangle;
        private NesEngine myEngine = new NesEngine();
        private IGameComponent s;
        private static FpsCounter FPS = null;
        public static StorageDevice saveDevice = null;
        private string[] roms;
        private bool waitForBackRelease = false;
        private bool running = false;
        private int topMenuRom;
        private int selectedRom;

        Object stateobj;

        public static FpsCounter FpsCounter {
            get {
                return FPS;
            }
        }

        public SharpNES() {

            graphics = new GraphicsDeviceManager( this );
            content = new ContentManager( Services );
            IsFixedTimeStep = false;
            s = new GamerServicesComponent( this );
            FPS = new FpsCounter( this );
            menuPad = new GamePadHelper( 20 );
            Components.Add( FPS );
            Components.Add( s );
            s.Initialize();

            if ( !Guide.IsVisible ) {
                // Reset the device
                saveDevice = null;
                stateobj = ( Object ) "GetDevice for Player One";
                Guide.BeginShowStorageDeviceSelector( PlayerIndex.One, this.GetDevice, stateobj );
            }

        }

        void GetDevice( IAsyncResult result ) {
            saveDevice = Guide.EndShowStorageDeviceSelector( result );
            if ( saveDevice.IsConnected ) {
                ArrayList files = new ArrayList();
                files.AddRange( Directory.GetFiles( StorageContainer.TitleLocation ) );
                int i = 0;
                while ( i < files.Count ) {
                    if ( files[i].ToString().ToLower().EndsWith( ".nes" ) )
                        i++;
                    else
                        files.RemoveAt( i );
                }
                roms = ( string[] ) files.ToArray( typeof( string ) );

                Array.Sort( roms );

                string lastRom = LoadRomName();
                selectedRom = Array.IndexOf( roms, lastRom );
                if ( selectedRom < 0 ) {
                    selectedRom = 0;
                }

                topMenuRom = 0;
            }
        }

        protected override void Initialize() {
            //For some reason LoadContent() Won't do the job :/
            spriteBatch = new SpriteBatch( graphics.GraphicsDevice );;
            screenRectangle = new Rectangle(
                30, 30,
                graphics.PreferredBackBufferWidth - 60,
                graphics.PreferredBackBufferHeight - 60
            );
            targetTexture = new Texture2D(
                graphics.GraphicsDevice,
                256, 224, 1,
                TextureUsage.None,
                SurfaceFormat.Bgr565
            );

            //Menu Stuff
            backgroundTexture = content.Load<Texture2D>( "bg" ) as Texture2D;
            helpBackgroundTexture = content.Load<Texture2D>( "help" ) as Texture2D;
            LoadFonts();
        }

        protected override void LoadContent() { }

        private void LoadFonts() {
            MenuItemHeight = 16 * 4 / 3;
            MenuPosition = new Vector2( graphics.PreferredBackBufferWidth / 8, 100 + MenuItemHeight );
            MenuSpriteFont = content.Load<SpriteFont>( "Verdana" );
        }


        protected override void UnloadContent() { }

        protected override void Update( GameTime gameTime ) {
            if ( running ) {
                UpdateGame();
            }
            else {
                UpdateMenu();
            }

            base.Update( gameTime );
        }

        protected override void Draw( GameTime gameTime ) {
            if ( running ) {
                DrawGame();
            }
            else {
                DrawMenu();
            }
            base.Draw( gameTime );
        }

        private void UpdateGame() {
            GamePadState pad = GamePad.GetState( PlayerIndex.One );

            if ( ( ( pad.Buttons.Back == ButtonState.Pressed ) &&
                ( pad.Buttons.RightShoulder == ButtonState.Pressed ) ) || Keyboard.GetState().IsKeyDown( Keys.Escape ) ) {
                StopCart();
                waitForBackRelease = true;
                return;
            }

            myEngine.RunCart();
        }

        private void DrawGame() {
            targetTexture.Dispose();
            
            targetTexture = new Texture2D ( 
                graphics.GraphicsDevice,
                256, 224, 1,
                TextureUsage.None,
                SurfaceFormat.Bgr565
            );

            targetTexture.SetData<short>(
                myEngine.myPPU.offscreenBuffer,
                256 * 8,
                targetTexture.Width * targetTexture.Height,
                SetDataOptions.None
            );

            spriteBatch.Begin( SpriteBlendMode.AlphaBlend );
            spriteBatch.Draw( targetTexture, screenRectangle, Color.White );
            DrawDebug();
            spriteBatch.End();
        }

        private void UpdateMenu() {
            menuPad.Update();
            GamePadState pad = GamePad.GetState( PlayerIndex.One );
            if ( waitForBackRelease ) {
                // Since the back button is used to leave the game,
                // we need to wait here for it to be released.
                if ( menuPad.BackIsPressed ) {
                    return;
                }

                menuPad.Reset();
                waitForBackRelease = false;
            }

            if ( menuPad.BackIsPressed ) {
                Quit();
            }

            if ( roms.Length > 0 ) {
                
                if ( menuPad.StartIsPressed || menuPad.AIsPressed || Keyboard.GetState().IsKeyDown( Keys.A ) ) {
                    LoadCart( roms[selectedRom] );
                }
                else if ( menuPad.UpWasPressed || menuPad.LeftWasPressed || Keyboard.GetState().IsKeyDown( Keys.Left ) || Keyboard.GetState().IsKeyDown( Keys.Up ) ) {
                    selectedRom = Math.Max( selectedRom - 1, 0 );

                    if ( selectedRom < topMenuRom ) {
                        topMenuRom--;
                    }
                }
                else if ( menuPad.DownWasPressed || menuPad.RightWasPressed || Keyboard.GetState().IsKeyDown( Keys.Down ) || Keyboard.GetState().IsKeyDown( Keys.Right ) ) {
                    selectedRom = Math.Min( selectedRom + 1, roms.Length - 1 );

                    if ( selectedRom >= topMenuRom + NumMenuItems ) {
                        topMenuRom++;
                    }
                }
            }
        }

        private void DrawDebug() {
            #if DEBUG
            spriteBatch.DrawString( MenuSpriteFont,
                FpsCounter.FPS.ToString() + "fps" + "\n",
                //"Real Resolution: " + graphics.PreferredBackBufferWidth + " x " + graphics.PreferredBackBufferHeight + "\n" +
                //"Rendered Resolution: " + Window.ClientBounds.Width + " x " + Window.ClientBounds.Height + "\n" +
                //"ROM Loaded: " + roms[selectedRom],
                new Vector2( 50, 50 ), Color.Black );
            #endif
        }

        private void DrawMenu() {
            GamePadState pad = GamePad.GetState( PlayerIndex.One );
            graphics.GraphicsDevice.Clear( BackgroundColor );
            spriteBatch.Begin( SpriteBlendMode.AlphaBlend );
            spriteBatch.Draw( backgroundTexture, new Vector2( 0, 0 ), Color.White );
            DrawDebug();
            if ( roms.Length == 0 ) {
                spriteBatch.DrawString( MenuSpriteFont, "No ROMs Available.", MenuPosition, Color.Black );
            }
            else {
                DrawRomNames();
            }
            if ( pad.Buttons.LeftShoulder == ButtonState.Pressed ) {
                DrawHelp();
            }
            spriteBatch.End();
        }

        private void DrawHelp() {
            spriteBatch.Draw( helpBackgroundTexture, new Vector2( 0, 0 ), Color.White );
        }

        private void DrawRomNames() {
            for ( int index = 0; index < NumMenuItems; index++ ) {
                int currentMenuRom = topMenuRom + index;
                Color itemColor = ( currentMenuRom == selectedRom ) ? SelectedItemColor : MenuItemColor;

                if ( roms.Length > currentMenuRom ) {
                    spriteBatch.DrawString(
                        MenuSpriteFont,
                        Path.GetFileNameWithoutExtension( roms[currentMenuRom] ),
                        new Vector2(
                            MenuPosition.X,
                            MenuPosition.Y + index * MenuItemHeight
                        ),
                        itemColor
                    );
                }
            }
        }

        private void SaveRomName( string rom ) {
            using ( StorageContainer container = saveDevice.OpenContainer( "SharpNES" ) ) {
                using ( StreamWriter writer = File.CreateText( Path.Combine( container.Path, "LastRom.txt" ) ) ) {
                    writer.Write( rom );
                }
            }
        }

        private string LoadRomName() {
            string result = String.Empty;

            using ( StorageContainer container = saveDevice.OpenContainer( "SharpNES" ) ) {
                string filename = Path.Combine( container.Path, "LastRom.txt" );

                if ( File.Exists( filename ) ) {
                    using ( StreamReader reader = File.OpenText( filename ) )
                        result = reader.ReadLine();
                }
            }

            return result;
        }

        private void LoadCart( string filename ) {
            SaveRomName( filename );
            myEngine.LoadCart( filename );
            myEngine.LoadRam();
            myEngine.StartCart();
            running = true;
        }

        private void StopCart() {
            using ( StorageContainer container = saveDevice.OpenContainer( "SharpNES" ) ) {
                myEngine.SaveRamDirectory = container.Path;
                myEngine.StopCart(); // Writes SaveRam
            }
            running = false;
        }

        private void Quit() {
            StopCart();
            this.Exit();
        }
    }
}