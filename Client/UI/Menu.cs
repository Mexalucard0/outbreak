﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;

namespace Outbreak
{
    class Menu : BaseScript
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int[] HeaderColor { get; set; } = new int[] { 16, 72, 144, 255 };
        public bool Visible { get; set; } = false;
        public int Index { get; set; } = 1;
        public int TitleFont { get; set; } = 1;
        public int DescriptionFont { get; set; } = 0;
        private int Select { get; set; } = 1;
        private float Rest { get; set; } = 0;
        internal Dictionary<int, string> ListOfItems { get; set; } = new Dictionary<int, string>();
        internal Dictionary<int, string> ListOfDescription { get; set; } = new Dictionary<int, string>();
        private static List<Menu> Menus = new List<Menu>();
        private int OnPressed { get; set; } = 0;
        public delegate void ItemSelectEvent(string name, int index);

        protected virtual void ItemSelectedEvent(string name, int index)
        {
            OnItemSelect.Invoke(name, index);
        }

        public event ItemSelectEvent OnItemSelect;

        public Menu(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public void AddItem(string item, string description)
        {
            ListOfItems.Add(Index, item);
            ListOfDescription.Add(Index, description);
            Index += 1;
        }

        public void Initiation()
        {

            if (Visible)
            {
                if (!Game.IsPaused && IsScreenFadedIn() && !IsPlayerSwitchInProgress() && !Game.PlayerPed.IsDead)
                {
                    Controller();
                    Interface();
                }
                else
                {
                    Visible = false;
                }
            }
        }

        public void Controller()
        {

            if (Game.IsControlPressed(0, Control.PhoneUp))
            {
                OnPressed += 1;

                if(OnPressed == 1 || OnPressed == 13)
                {

                    if (Select > 1)
                    {
                        Select -= 1;
                        PlaySoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);

                        if (Rest > 0f)
                        {
                            Rest -= 1f;
                        }
                    }

                    OnPressed = 5;
                }
            }
            else if (Game.IsControlPressed(0, Control.PhoneDown))
            {
                OnPressed += 1;

                if (OnPressed == 1 ||  OnPressed == 13)
                {
                    if (Select < Index - 1)
                    {
                        Select += 1;
                        PlaySoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false); //NAV_UP_DOWN

                        if (Select > 8)
                        {
                            Rest += 1f;
                        }
                    }

                    OnPressed = 5;
                }
            }
            else if (Game.IsControlJustPressed(0, Control.FrontendRdown))
            {
                ItemSelectedEvent(ListOfItems[Select], Select);
                PlaySoundFrontend(-1, "OK", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
            }
            else if (Game.IsControlJustPressed(0, Control.FrontendRdown) || Game.IsControlJustPressed(0, Control.FrontendCancel))
            {
                Visible = false;
                PlaySoundFrontend(-1, "BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET", false); //EXIT //	QUIT //CANCEL
            }
            else
            {
                OnPressed = 0;
            }
        }

        public void Interface()
        {
            float TitleX = 0.11f;
            float TitleY = 0.06f;
            float TitleWidth = 0.23f;
            float TitleHeight = 0.1f;

            //////////////////// Header ////////////////////

            SetScriptGfxAlign(82, 84);
            SetScriptGfxAlignParams(0f, 0f, 0f, 0f);
            DrawRect(TitleX, TitleY, TitleWidth, TitleHeight, HeaderColor[0], HeaderColor[1], HeaderColor[2], HeaderColor[3]);
            BeginTextCommandDisplayText("STRING");
            SetTextFont(TitleFont);
            SetTextColour(255, 255, 255, 255);
            SetTextScale(1f, 1f);
            SetTextJustification(0);
            AddTextComponentSubstringPlayerName(Title);
            EndTextCommandDisplayText(TitleX + 0.77f, TitleY - 0.027f);
            ResetScriptGfxAlign();

            //////////////////// Description ////////////////////

            SetScriptGfxAlign(82, 84);
            SetScriptGfxAlignParams(0f, 0f, 0f, 0f);
            DrawRect(TitleX, TitleY + 0.067f, TitleWidth, TitleHeight / 3f, 0, 0, 0, 250);
            BeginTextCommandDisplayText("STRING");
            SetTextFont(DescriptionFont);
            SetTextScale(0.32f, 0.32f);
            SetTextJustification(1);
            AddTextComponentSubstringPlayerName("~w~" + Description.ToUpper());
            EndTextCommandDisplayText(TitleX + 0.658f, TitleY + 0.054f);
            ResetScriptGfxAlign();

            //////////////////// ItemCount ////////////////////

            SetScriptGfxAlign(82, 84);
            SetScriptGfxAlignParams(0f, 0f, 0f, 0f);
            BeginTextCommandDisplayText("STRING");
            SetTextFont(0);
            SetTextScale(0.32f, 0.32f);
            SetTextJustification(2);
            AddTextComponentSubstringPlayerName($"{Select} / {Index - 1}");
            SetTextWrap(-0.01f, TitleY + 0.054f);
            EndTextCommandDisplayText(0f, TitleY + 0.054f);
            ResetScriptGfxAlign();

            //////////////////// Items ////////////////////

            SetScriptGfxAlign(82, 84);
            SetScriptGfxAlignParams(0f, 0f, 0f, 0f);

            for (float i = 1; i <= (Index-1); i++)
            {
                if (Select == i)
                {
                    BeginTextCommandDisplayText("STRING");
                    SetTextFont(0);
                    SetTextScale(0.35f, 0.35f);
                    SetTextJustification(1);
                    AddTextComponentSubstringPlayerName($"~u~{ListOfItems[Select]}");

                    if (Select > 8)
                    {
                        DrawRect(TitleX, (TitleY + 0.066f) + (0.0357f * (i - Rest)), TitleWidth, TitleHeight / 2.8f, 255, 255, 255, 180);
                        EndTextCommandDisplayText(TitleX + 0.658f, (TitleY + 0.052f) + (0.0357f * (i - Rest)));
                    }
                    else
                    {
                        DrawRect(TitleX, (TitleY + 0.066f) + (0.0357f * i), TitleWidth, TitleHeight / 2.8f, 255, 255, 255, 180);
                        EndTextCommandDisplayText(TitleX + 0.658f, (TitleY + 0.052f) + (0.0357f * i));
                    }
                }
                else if (i <= 8)
                {
                    BeginTextCommandDisplayText("STRING");
                    SetTextFont(0);
                    SetTextScale(0.35f, 0.35f);
                    SetTextJustification(1);
                    AddTextComponentSubstringPlayerName($"{ListOfItems[(int)i + (int)Rest]}");
                    EndTextCommandDisplayText(TitleX + 0.658f, (TitleY + 0.052f) + (0.0357f * i));
                    if ( i <= 7 && Select > 8)
                    {
                        DrawRect(TitleX, (TitleY + 0.066f) + (0.0357f * (i)), TitleWidth, TitleHeight / 2.8f, 0, 0, 0, 180);
                    }
                    else if (i != Select && Select <= 8)
                    {
                        DrawRect(TitleX, (TitleY + 0.066f) + (0.0357f * (i)), TitleWidth, TitleHeight / 2.8f, 0, 0, 0, 180);
                    }
                }
            }

            ResetScriptGfxAlign();

            //////////////////// Indicator ////////////////////

            if (Index > 7)
            {
                SetScriptGfxAlign(82, 84);
                SetScriptGfxAlignParams(0f, 0f, 0f, 0f);
                DrawRect(TitleX, TitleY + 0.39f, TitleWidth, 0.03f, 0, 0, 0, 200);

                if (Select > 8)
                {
                    BeginTextCommandDisplayText("STRING");
                    AddTextComponentSubstringPlayerName("↑");

                    SetTextFont(0);
                    SetTextScale(0.4f, 0.4f);
                    SetTextJustification(0);

                    EndTextCommandDisplayText(TitleX + 0.77f, TitleY + 0.372f);
                }

                if (Select < (Index - 1))
                {
                    BeginTextCommandDisplayText("STRING");
                    AddTextComponentSubstringPlayerName("↓");

                    SetTextFont(0);
                    SetTextScale(0.4f, 0.4f);
                    SetTextJustification(0);
                    EndTextCommandDisplayText(TitleX + 0.77f, TitleY + 0.378f);
                }

                ResetScriptGfxAlign();
            }

            //////////////////// Item Description ////////////////////

            SetScriptGfxAlign(82, 84);
            SetScriptGfxAlignParams(0f, 0f, 0f, 0f);
            BeginTextCommandDisplayText("STRING");
            SetTextFont(0);
            SetTextScale(0.30f, 0.30f);
            SetTextJustification(1);

            for (float i = 1; i <= (Index - 1); i++)
            {
                if (Select == i)
                {
                    AddTextComponentSubstringPlayerName(ListOfDescription[Select]);
                }
            }

            if ((Index -1) > 8)
            {
                SetTextWrap(-0.01f, 0.99f);
                EndTextCommandDisplayText(TitleX + 0.658f, (TitleY + 0.13f) + (0.0357f * (8f)));
                DrawRect(TitleX, (TitleY + 0.152f) + (0.0357f * (8f)), TitleWidth, 0.05f, 0, 0, 0, 200);
            }
            else
            {
                EndTextCommandDisplayText(TitleX + 0.658f, (TitleY + 0.1f) + (0.0357f * (Index - 1)));
                DrawRect(TitleX, (TitleY + 0.122f) + (0.0357f * (Index - 1)), TitleWidth, 0.05f, 0, 0, 0, 200);
            }

            ResetScriptGfxAlign();
        }

        public bool IsAnyMenuOpen()
        {
            return Menus.Any(m => m.Visible);
        }

        public void Register(Menu name)
        {
            Menus.Add(name);
        }

        public void OpenMenu()
        {
            Visible = !Visible;
            PlaySoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
        }
    }
}
