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
        #endregion

        public bool StartWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.Start, ref startWasPressed);
            }
        }

        public bool BackWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.Back, ref backWasPressed);
            }
        }

        public bool UpWasPressed
        {
            get
            {
                return CheckPressed(currentState.DPad.Up, ref upWasPressed);
            }
        }

        public bool AWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.A, ref aWasPressed);
            }
        }

        public bool BWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.B, ref bWasPressed);
            }
        }

        public bool XWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.X, ref xWasPressed);
            }
        }

        public bool YWasPressed
        {
            get
            {
                return CheckPressed(currentState.Buttons.Y, ref yWasPressed);
            }
        }

        public bool DownWasPressed
        {
            get
            {
                return CheckPressed(currentState.DPad.Down, ref downWasPressed);
            }
        }

        public bool LeftWasPressed
        {
            get
            {
                return CheckPressed(currentState.DPad.Left, ref leftWasPressed);
            }
        }

        public bool RightWasPressed
        {
            get
            {
                return CheckPressed(currentState.DPad.Right, ref rightWasPressed);
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

        /// <summary>
        /// Constructs a GamePadHelper object for Player 1.
        /// </summary>
        public GamePadHelper()
        {
            this.player = PlayerIndex.One;
        }

        /// <summary>
        /// Constructs a GamePadHelper object for given Player.
        /// </summary>
        /// <param name="player">Player index</param>
        public GamePadHelper(PlayerIndex player)
        {
            this.player = player;
        }

        /// <summary>
        /// Updates the state of all gamepad buttons.
        /// Must be called every frame.
        /// </summary>
        public void Update()
        {
            currentState = GamePad.GetState(PlayerIndex.One);
            UpdateButtonState(currentState.Buttons.Start, ref startWasPressed);
            UpdateButtonState(currentState.Buttons.Back, ref backWasPressed);
            UpdateButtonState(currentState.Buttons.A, ref aWasPressed);
            UpdateButtonState(currentState.Buttons.B, ref bWasPressed);
            UpdateButtonState(currentState.Buttons.X, ref xWasPressed);
            UpdateButtonState(currentState.Buttons.Y, ref yWasPressed);
            UpdateButtonState(currentState.DPad.Up, ref upWasPressed);
            UpdateButtonState(currentState.DPad.Down, ref downWasPressed);
            UpdateButtonState(currentState.DPad.Left, ref leftWasPressed);
            UpdateButtonState(currentState.DPad.Right, ref rightWasPressed);
        }

        #region Private Methods
        private void UpdateButtonState(ButtonState state, ref bool wasPressed)
        {
            if (state == ButtonState.Pressed)
            {
                wasPressed = true;
            }
        }

        private bool CheckPressed(ButtonState state, ref bool wasPressed)
        {
            if (wasPressed && state == ButtonState.Released)
            {
                wasPressed = false;
                return true;
            }
            return false;
        }
        #endregion
    }
}
