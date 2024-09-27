using UnityEngine;
using System.Collections.Generic;
using VRC.SDK3.Avatars.Components;

using com.mitsukaki.poseengine.editor.ui.Components;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace com.mitsukaki.poseengine.editor.ui.Views
{
    public class EditPanelView : VisualElement
    {
        private VisualElement avatarField;
        private VisualElement introView;
        private VisualElement editView;

        private VRCAvatarDescriptor selectedAvatar;

        public EditPanelView()
        {
            introView = CreateIntroView();
            editView = CreateEditView();

            Show(introView);
            Hide(editView);

            Add(introView);
            Add(GetAvatarSelector());
            Add(editView);
        }

        private void Show(VisualElement view)
        {
            view.style.display = DisplayStyle.Flex;
        }

        private void Hide(VisualElement view)
        {
            view.style.display = DisplayStyle.None;
        }

        private VisualElement GetAvatarSelector()
        {
            if (avatarField != null) return avatarField;
            avatarField = new VisualElement();

            var inputLabel = new Label("Selected Avatar");
            inputLabel.AddToClassList("pe_avatar_label");

            var avatarInput = new ObjectField("")
            {
                objectType = typeof(VRCAvatarDescriptor),
                allowSceneObjects = true
            };

            avatarInput.RegisterValueChangedCallback(evt =>
            {
                OnAvatarSelected(evt.newValue as VRCAvatarDescriptor);
            });

            avatarInput.AddToClassList("pe_avatar_selector");

            avatarField.Add(inputLabel);
            avatarField.Add(avatarInput);

            return avatarField;
        }

        private void Recompute()
        {
            // if an avatar is selected, show the edit view
            if (selectedAvatar != null)
            {
                Show(editView);
                Hide(introView);
            }
            else
            {
                Show(introView);
                Hide(editView);
            }
        }

        private GameObject GetAvatarRoot()
        {
            if (selectedAvatar == null) return null;

            return selectedAvatar.gameObject;
        }

        private void OnAvatarSelected(VRCAvatarDescriptor avatar)
        {
            selectedAvatar = avatar;

            Recompute();
        }

        private VisualElement CreateIntroView()
        {
            VisualElement setupView = new VisualElement();

            Label setupLabel = new Label("Setup");
            setupLabel.AddToClassList("pe_header-label");
            setupView.Add(setupLabel);

            Label setupNote = new Label("Select an avatar from the scene to begin editing.");
            setupNote.AddToClassList("pe_paragraph");
            setupNote.AddToClassList("pe_setup-note");
            setupView.Add(setupNote);

            return setupView;
        }

        private VisualElement CreateEditView()
        {
            VisualElement editView = new VisualElement();

            editView.Add(GetAvatarSelector());

            Label editLabel = new Label("Edit");
            editLabel.AddToClassList("pe_header-label");

            editView.Add(editLabel);

            return editView;
        }
    }
}