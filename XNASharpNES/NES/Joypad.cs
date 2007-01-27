// created on 11/27/2004 at 9:32 AM
using System;
using System.IO;
//using Tao.Sdl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class Joypad
{
    enum Button
    {
        BUTTON_A = 1,
        BUTTON_B = 2,
        BUTTON_SELECT = 4,
        BUTTON_START = 8,
        BUTTON_UP = 16,
        BUTTON_DOWN = 32,
        BUTTON_LEFT = 64,
        BUTTON_RIGHT = 128
    };

    byte joypad1_lastwrite;
    byte joypad2_lastwrite;
    int joypad1_readpointer;
    int joypad2_readpointer;

    byte joypad1_state;

    private void InternalGetJoyState()
    {
        //int numberOfKeys;
        ////FIXME: This is SDL-centric for the time being

        //byte[] keystate = Sdl.SDL_GetKeyState(out numberOfKeys);

        //joypad1_state = 0;

        //if (keystate[(int)Sdl.SDLKey.SDLK_z] != 0)
        //    joypad1_state |= (byte)Button.BUTTON_A;
        //if (keystate[(int)Sdl.SDLKey.SDLK_x] != 0)
        //    joypad1_state |= (byte)Button.BUTTON_B;
        //if (keystate[(int)Sdl.SDLKey.SDLK_a] != 0)
        //    joypad1_state |= (byte)Button.BUTTON_SELECT;
        //if (keystate[(int)Sdl.SDLKey.SDLK_s] != 0)
        //    joypad1_state |= (byte)Button.BUTTON_START;
        //if (keystate[(int)Sdl.SDLKey.SDLK_UP] != 0)
        //    joypad1_state |= (byte)Button.BUTTON_UP;
        //else if (keystate[(int)Sdl.SDLKey.SDLK_DOWN] != 0)
        //    joypad1_state |= (byte)Button.BUTTON_DOWN;
        //if (keystate[(int)Sdl.SDLKey.SDLK_LEFT] != 0)
        //    joypad1_state |= (byte)Button.BUTTON_LEFT;
        //else if (keystate[(int)Sdl.SDLKey.SDLK_RIGHT] != 0)
        //    joypad1_state |= (byte)Button.BUTTON_RIGHT;

        joypad1_state = 0;

        GamePadState pad1 = GamePad.GetState(PlayerIndex.One);
        if (pad1.Buttons.A == ButtonState.Pressed)
            joypad1_state |= (byte)Button.BUTTON_A;
        if (pad1.Buttons.B == ButtonState.Pressed || pad1.Buttons.X == ButtonState.Pressed)
            joypad1_state |= (byte)Button.BUTTON_B;
        if (pad1.Buttons.Back == ButtonState.Pressed)
            joypad1_state |= (byte)Button.BUTTON_SELECT;
        if (pad1.Buttons.Start == ButtonState.Pressed)
            joypad1_state |= (byte)Button.BUTTON_START;
        if (pad1.DPad.Up == ButtonState.Pressed)
            joypad1_state |= (byte)Button.BUTTON_UP;
        else if (pad1.DPad.Down == ButtonState.Pressed)
            joypad1_state |= (byte)Button.BUTTON_DOWN;
        if (pad1.DPad.Left == ButtonState.Pressed)
            joypad1_state |= (byte)Button.BUTTON_LEFT;
        else if (pad1.DPad.Right == ButtonState.Pressed)
            joypad1_state |= (byte)Button.BUTTON_RIGHT;
    }
    
    public byte Joypad_1_Read()
    {
        byte returnedValue = 0;

        switch (joypad1_readpointer)
        {
            case (1): if ((joypad1_state & (byte)Button.BUTTON_A) == (byte)Button.BUTTON_A) { returnedValue = 1; }; break;
            case (2): if ((joypad1_state & (byte)Button.BUTTON_B) == (byte)Button.BUTTON_B) { returnedValue = 1; }; break;
            case (3): if ((joypad1_state & (byte)Button.BUTTON_SELECT) == (byte)Button.BUTTON_SELECT) { returnedValue = 1; }; break;
            case (4): if ((joypad1_state & (byte)Button.BUTTON_START) == (byte)Button.BUTTON_START) { returnedValue = 1; }; break;
            case (5): if ((joypad1_state & (byte)Button.BUTTON_UP) == (byte)Button.BUTTON_UP) { returnedValue = 1; }; break;
            case (6): if ((joypad1_state & (byte)Button.BUTTON_DOWN) == (byte)Button.BUTTON_DOWN) { returnedValue = 1; }; break;
            case (7): if ((joypad1_state & (byte)Button.BUTTON_LEFT) == (byte)Button.BUTTON_LEFT) { returnedValue = 1; }; break;
            case (8): if ((joypad1_state & (byte)Button.BUTTON_RIGHT) == (byte)Button.BUTTON_RIGHT) { returnedValue = 1; }; break;
        }
        joypad1_readpointer++;
        return returnedValue;
    }
    
    public byte Joypad_2_Read()
    {
        return 0;
    }
 
    public void Joypad_1_Write(byte data)
    {
        if ((data == 0) && (joypad1_lastwrite == 1))
        {
            InternalGetJoyState();
            joypad1_readpointer = 1;
        }
        joypad1_lastwrite = data;
    }

    public void Joypad_2_Write(byte data)
    {
    }
}