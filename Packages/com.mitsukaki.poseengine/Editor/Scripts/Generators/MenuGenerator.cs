
using UnityEngine;
using UnityEditor;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

using nadena.dev.modular_avatar.core;

namespace com.mitsukaki.poseengine.editor.generators
{
    public class MenuGenerator : IPoseGenerator
    {
        public void Setup(PoseBuildContext context)
        {
            var poseContainer = context.poseEngineInstance.transform
                .GetChild(1).Find("Poses");
        
            // Get all the AGeneratorMenu components
            var menuComponents = context.avatarRoot.GetComponentsInChildren<AGeneratorMenu>();

            // give all the poses an unique ID
            int poseIndex = 1;
            foreach (var menuComp in menuComponents)
                poseIndex = menuComp.EnumeratePoses(poseIndex);

            // Get all PEGeneratorMenu components
            var peMenuComponents = context.avatarRoot
                .GetComponentsInChildren<PEMenu>();

            // migrate all the pose generator's data components into the poses game object, maintaining their heirarchy
            // if they're on the same object as a PEMenu object then they are transferred to the root of the poses game object.
            foreach (var menuComp in peMenuComponents)
            {
                // copy any AGeneratorMenu components off the PEMenu game object to the poses game object
                foreach (var menu in menuComp.GetComponents<AGeneratorMenu>())
                {
                    menu.CopyTo(poseContainer.gameObject);
                }

                // make a copy of each child in the poses game object
                foreach (Transform child in menuComp.transform)
                {
                    var copy = GameObject.Instantiate(child.gameObject, poseContainer);
                    copy.name = child.name;
                }
            }

            // create the menus for the pose hierarchy
            CreateMenuFolders(context, poseContainer.gameObject);

            // create the pose toggles for the pose container
            CreatePoseToggles(context, poseContainer.gameObject);

            // save asset
            AssetDatabase.SaveAssets();
        }

        public void BuildLayers(PoseBuildContext context)
        {
            // No layers to build
        }

        public void BuildStates(PoseBuildContext context)
        {
            // No states to build
        }

        public void CreateMenuFolders(PoseBuildContext context, GameObject menu)
        {
            // add menu item script to the object
            var menuItem = menu.AddComponent<ModularAvatarMenuItem>();
            menuItem.Control = new VRCExpressionsMenu.Control();
            menuItem.MenuSource = SubmenuSource.Children;

            // create the menu item for the object
            menuItem.Control.name = menu.name;
            menuItem.Control.type = VRCExpressionsMenu.Control.ControlType.SubMenu;

            // if the object has a PEMenuFolder component, grab the icon and name
            var menuFolder = menu.GetComponent<PEMenuFolder>();
            if (menuFolder != null)
            {
                menuItem.Control.icon = menuFolder.Icon;
                menuItem.Control.name = menuFolder.Name;
            }

            // create a menu on every child object
            foreach (Transform child in menu.transform)
                CreateMenuFolders(context, child.gameObject);
        }

        public void CreatePoseToggles(PoseBuildContext context, GameObject menu)
        {
            // Get all AGeneratorMenu components
            var menuComponents = menu.GetComponentsInChildren<AGeneratorMenu>();
            
            // for each menu component
            foreach (var menuComp in menuComponents)
            {
                var obj = menuComp.gameObject;

                // create a child for every pose
                foreach (var pose in menuComp.GetPoseList())
                {
                    // if we aren't supposed to make a menu, skip this object
                    if (pose.MenuControlType == PoseMenuControlType.None)
                        continue;

                    // create a child game object for the pose
                    var poseObject = new GameObject(pose.Name);
                    poseObject.transform.parent = obj.transform;

                    // attatch the menu item to the object
                    var poseMenuItem = poseObject.AddComponent<ModularAvatarMenuItem>();
                    poseMenuItem.Control = new VRCExpressionsMenu.Control();

                    // create the menu item for the object
                    poseMenuItem.Control.name = pose.Name;
                    poseMenuItem.Control.icon = pose.Icon;
                    poseMenuItem.Control.value = pose.PoseID;

                    switch(pose.MenuControlType)
                    {
                        case PoseMenuControlType.ToggleEnable:
                            poseMenuItem.Control.type = VRCExpressionsMenu.Control.ControlType.Toggle;
                            break;

                        case PoseMenuControlType.Radial:
                            poseMenuItem.Control.type = VRCExpressionsMenu.Control.ControlType.RadialPuppet;
                            poseMenuItem.Control.subParameters = new VRCExpressionsMenu.Control.Parameter[1];
                            poseMenuItem.Control.subParameters[0] = new VRCExpressionsMenu.Control.Parameter();
                            poseMenuItem.Control.subParameters[0].name = pose.DrivingParameterName;

                            break;
                    }

                    // add parameter to the menu item
                    poseMenuItem.Control.parameter = new VRCExpressionsMenu.Control.Parameter();
                    poseMenuItem.Control.parameter.name = "PoseEngine/Pose";
                }
            }
        }

        public void CleanUp(PoseBuildContext context)
        {
            // No cleanup to do
        }
    }
}