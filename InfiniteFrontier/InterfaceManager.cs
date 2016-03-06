using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class InterfaceManager
    {
        public GUI_Map GUIMap { get; set; }
        public GUI_Overlay GUIOverlay { get; set; }
        public GUI_PlanetManagment GUIPlanetManagment { get; set; }
        public GUI_SocialPolicy GUISocialPolicy { get; set; }
        public GUI_Tech GUITech { get; set; }
        public GUI_Diplomacy GUIDiplomacy { get; set; }
        public GUI_GameMenu GUIGameMenu { get; set; }
        public GUI_UnitManagment GUIUnitManagment { get; set; }
        public GUI_UnitList GUIUnitList { get; set; }
        public GUI_PlanetList GUIPlanetList { get; set; }
        public GUI_FleetList GUIFleetList { get; set; }
        public GUI_FleetTemplateList GUIFleetTemplateList { get; set; }
        public GUI_FleetDesigner GUIFleetDesigner { get; set; }

        private List<IGUIElement> guiElements = new List<IGUIElement>();

        public InterfaceManager()
        {
            GUIMap = new GUI_Map();
            GUIOverlay = new GUI_Overlay();
            GUIPlanetManagment = new GUI_PlanetManagment();
            GUISocialPolicy = new GUI_SocialPolicy();
            GUITech = new GUI_Tech();
            GUIDiplomacy = new GUI_Diplomacy();
            GUIGameMenu = new GUI_GameMenu();
            GUIUnitManagment = new GUI_UnitManagment();
            GUIUnitList = new GUI_UnitList();
            GUIPlanetList = new GUI_PlanetList();
            GUIFleetList = new GUI_FleetList();
            GUIFleetTemplateList = new GUI_FleetTemplateList();
            GUIFleetDesigner = new GUI_FleetDesigner();

            guiElements.Add(GUIOverlay);
            guiElements.Add(GUIMap);
            guiElements.Add(GUIPlanetManagment);
            guiElements.Add(GUISocialPolicy);
            guiElements.Add(GUITech);
            guiElements.Add(GUIUnitManagment);
            guiElements.Add(GUIUnitList);
            guiElements.Add(GUIPlanetList);
            guiElements.Add(GUIFleetList);
            guiElements.Add(GUIFleetTemplateList);
            guiElements.Add(GUIFleetDesigner);
        }

        public void CloseForms()
        {
            foreach (IGUIElement gui in guiElements)
                if (gui.Form != null)
                    gui.Form.Hidden = true;
        }

        public bool Update(GameTime gameTime)
        {
            if (InputUtil.WasKeyDown(Keys.Escape))
                CloseForms();

            bool interacted = false;
            GUIGameMenu.Update(gameTime, false);
            if (!GUIGameMenu.Form.Hidden)
            {
                CloseForms();
                simulationManager.Paused = true;
            }
            else
            {
                simulationManager.Paused = false;
                foreach (IGUIElement gui in guiElements)
                {
                    if (gui.Update(gameTime, interacted))
                        interacted = true;
                }
            }
            return interacted;
        }

        public void Draw()
        {
            for (int i = guiElements.Count - 1; i > -1; i--)
            {
                guiElements[i].Draw();                
            }
            if (settingsForm.Form.Hidden) 
                GUIGameMenu.Draw();
        }
    }
}
