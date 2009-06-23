using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNASharpNES
{
    public class GamePadHelper
    {
        #region Private Attributes
        private PlayerIndex player;
        private GamePadState currentState;
        private KeyboardState kb;
        private bool useAutoRepeat;
        private int autoRepeatCount;
        private bool startWasPressed = false;
        private bool backWasPressed = false;
        private bool aWasPressed = false;
        private bool bWasPressed = false;
        private bool xWasPressed = false;
        private bool yWasPressed = false;
        private bool upWasPressed = false;
        private bool downWasPressed = false;
        private bool leftWasPressed = false;
        private bool rightWasPressed = false;

        private bool kbaWasPressed = false;
        private bool kbsWasPressed = false;

        private int startRepeatCount = 0;
        private int backRepeatCount = 0;
        private int aRepeatCount = 0;
        private int bRepeatCount = 0;
        private int xRepeatCount = 0;
        private int yRepeatCount = 0;
        private int upRepeatCount = 0;
        private int downRepeatCount = 0;
        private int leftRepeatCount = 0;
        private int rightRepeatCount = 0;
        #endregion

        public bool KBAWasPressed {
            get {
                if ( kb.IsKeyDown( Keys.A ) )
                    return true;
                else
                    return false;
            }
        }

        public bool KBSWasPressed {
            get {
                if ( kb.IsKeyDown( Keys.S ) )
                    return true;
                else
                    return false;
            }
        }

        public bool StartWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.Start, ref startWasPressed, ref startRepeatCount);
            }
        }

        public bool BackWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.Back, ref backWasPressed, ref backRepeatCount);
            }
        }

        public bool AWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.A, ref aWasPressed, ref aRepeatCount);
            }
        }

        public bool BWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.B, ref bWasPressed, ref bRepeatCount);
            }
        }

        public bool XWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.X, ref xWasPressed, ref xRepeatCount);
            }
        }

        public bool YWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.Y, ref yWasPressed, ref yRepeatCount);
            }
        }

        public bool UpWasPressed
        {
            get
            {
                return CheckPressed(currentState.DPad.Up, ref upWasPressed, ref upRepeatCount);
            }
        }

        public bool DownWasPressed
        {
            get
            {
                return CheckPressed(currentState.DPad.Down, ref downWasPressed, ref downRepeatCount);
            }
        }

        public bool LeftWasPressed
        {
            get
            {
                return CheckPressed(currentState.DPad.Left, ref leftWasPressed, ref leftRepeatCount);
            }
        }

        public bool RightWasPressed
        {
            get
            {
                return CheckPressed(currentState.DPad.Right, ref rightWasPressed, ref rightRepeatCount);
            }
        }

        public bool StartIsPressed
        {
            get
            {
                return (currentState.Buttons.Start == ButtonState.Pressed);
            }
        }

        public bool BackIsPressed
        {
            get
            {
                return (currentState.Buttons.Back == ButtonState.Pressed);
            }
        }

        public bool AIsPressed
        {
            get
            {
                return (currentState.Buttons.A == ButtonState.Pressed);
            }
        }

        public bool BIsPressed
        {
            get
            {
                return (currentState.Buttons.B == ButtonState.Pressed);
            }
        }

        public bool XIsPressed
        {
            get
            {
                return (currentState.Buttons.X == ButtonState.Pressed);
            }
        }

        public bool YIsPressed
        {
            get
            {
                return (currentState.Buttons.Y == ButtonState.Pressed);
            }
        }

        public bool UpIsPressed
        {
            get
            {
                return (currentState.DPad.Up == ButtonState.Pressed);
            }
        }

        public bool DownIsPressed
        {
            get
            {
                return (currentState.DPad.Down == ButtonState.Pressed);
            }
        }

        public bool LeftIsPressed
        {
            get
            {
                return (currentState.DPad.Left == ButtonState.Pressed);
            }
        }

        public bool RightIsPressed
        {
            get
            {
                return (currentState.DPad.Right == ButtonState.Pressed);
            }
        }

        public bool RightShoulderIsPressed
        {
            get
            {
                return (currentState.Buttons.RightShoulder == ButtonState.Pressed);
            }
        }

        public bool LeftShoulderIsPressed
        {
            get
            {
                return (currentState.Buttons.LeftShoulder == ButtonState.Pressed);
            }
        }

        public bool KBAIsPressed {
            get {
                if ( kb.IsKeyDown( Keys.A ) )
                    return true;
                else
                    return false;
            }
        }

        public bool KBSIsPressed {
            get {
                if ( kb.IsKeyDown( Keys.S ) )
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Constructs a GamePadHelper object for Player 1.
        /// </summary>
        public GamePadHelper()
        {
            Init(PlayerIndex.One, 0);
        }

        /// <summary>
        /// Constructs a GamePadHelper object for given Player.
        /// </summary>
        /// <param name="autoRepeatCount">Allows a button to "repeat" if held down for greater than this number of updates.</param>
        public GamePadHelper(int autoRepeatCount)
        {
            Init(PlayerIndex.One, autoRepeatCount);
        }

        /// <summary>
        /// Constructs a GamePadHelper object for given Player.
        /// </summary>
        /// <param name="player">Player index</param>
        /// <param name="autoRepeatCount">Allows a button to "repeat" if held down for greater than this number of updates.</param>
        public GamePadHelper(int autoRepeatCount, PlayerIndex player)
        {
            Init(player, autoRepeatCount);
        }

        private void Init(PlayerIndex player, int autoRepeatCount)
        {
            this.player = player;
            this.autoRepeatCount = autoRepeatCount;
            useAutoRepeat = (autoRepeatCount != 0);
        }

        /// <summary>
        /// Updates the state of all gamepad buttons.
        /// Must be called every frame.
        /// </summary>
        public void Update()
        {
            currentState = GamePad.GetState(PlayerIndex.One);

            UpdateButtonState(currentState.Buttons.Start, ref startWasPressed, ref startRepeatCount);
            UpdateButtonState(currentState.Buttons.Back, ref backWasPressed, ref backRepeatCount);
            UpdateButtonState(currentState.Buttons.A, ref aWasPressed, ref aRepeatCount);
            UpdateButtonState(currentState.Buttons.B, ref bWasPressed, ref bRepeatCount);
            UpdateButtonState(currentState.Buttons.X, ref xWasPressed, ref xRepeatCount);
            UpdateButtonState(currentState.Buttons.Y, ref yWasPressed, ref yRepeatCount);
            UpdateButtonState(currentState.DPad.Up, ref upWasPressed, ref upRepeatCount);
            UpdateButtonState(currentState.DPad.Down, ref downWasPressed, ref downRepeatCount);
            UpdateButtonState(currentState.DPad.Left, ref leftWasPressed, ref leftRepeatCount);
            UpdateButtonState(currentState.DPad.Right, ref rightWasPressed, ref rightRepeatCount);

            /*if ( Keyboard.GetState().IsKeyDown( Keys.A ) ) {
                StartIsPressed = true;
                startRepeatCount = 20;
            }*/
        }

        /// <summary>
        /// Reset all state variables.
        /// </summary>
        public void Reset()
        {
            startWasPressed = false;
            backWasPressed = false;
            aWasPressed = false;
            bWasPressed = false;
            xWasPressed = false;
            yWasPressed = false;
            upWasPressed = false;
            downWasPressed = false;
            leftWasPressed = false;
            rightWasPressed = false;
        }

        #region Private Methods
        private void UpdateButtonState(ButtonState state, ref bool wasPressed, ref int repeatCounter)
        {
            if (state == ButtonState.Pressed)
            {
                if (!wasPressed)
                {
                    wasPressed = true;
                    repeatCounter = 0;
                }
                else
                {
                    if (useAutoRepeat)
                    {
                        repeatCounter = (repeatCounter + 1) % autoRepeatCount;
                    }
                }
            }
        }

        private bool CheckPressed(ButtonState state, ref bool wasPressed, ref int repeatCounter)
        {
            if (wasPressed)
            {
                if (state == ButtonState.Released)
                {
                    wasPressed = false;

                    if (!useAutoRepeat)
                    {
                        // When not using autorepeat, return true whenever button
                        // has been pressed and then released.
                        return true;
                    }
                }
                else if ((useAutoRepeat) && (repeatCounter == 0))
                {
                    // When using autorepeat, only return true when counter is 0.
                    return true;
                }
            }

            // For all other conditions, return false.
            return false;
        }
        #endregion
    }
}
