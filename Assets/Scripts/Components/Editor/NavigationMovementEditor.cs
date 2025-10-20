﻿#region Copyright
// MIT License
// 
// Copyright (c) 2023 David María Arribas
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using Navigation.Interfaces;
using Tools;
using UnityEditor;

namespace Components.Editor
{
    [CustomEditor(typeof(NavigationMovement))]
    public class NavigationMovementEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script", 
                "navigationClass", "navigationAgent");

            string className = serializedObject.FindProperty("navigationAgent").stringValue;
            serializedObject.FindProperty("navigationAgent").stringValue = 
                ShowClassSelector( "Navigation Agent", typeof(INavigationAgent), className);
            
            className = serializedObject.FindProperty("navigationClass").stringValue;
            serializedObject.FindProperty("navigationClass").stringValue = 
                ShowClassSelector( "Navigation Class", typeof(INavigationAlgorithm), className);

            
            serializedObject.ApplyModifiedProperties();
        }

        private string ShowClassSelector(String label, Type baseType, string currentClass)
        {
            List<String> classes = baseType.Subclasses();
            
            string[] options = classes.ToArray();
            int classIndex = 0;

            if (currentClass != null)
            {
                classIndex = Array.IndexOf(options, currentClass);
                classIndex = classIndex < 0 ? 0 : classIndex;
            }

            classIndex = EditorGUILayout.Popup(label, classIndex, options);
            
            string selectedClass = options[classIndex];
            return selectedClass;
        }
    }
}