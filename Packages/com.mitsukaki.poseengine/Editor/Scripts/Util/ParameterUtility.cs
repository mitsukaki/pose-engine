#region

using UnityEngine;
using UnityEditor;
using nadena.dev.modular_avatar.core;
using System;

#endregion

namespace com.mitsukaki.poseengine.editor
{
    public static class ParameterUtility
    {
        public static ParameterConfig FindParameterByName(
            ModularAvatarParameters maParams, string parameterName
        )
        {
            // iterate over all the parameters and check if the parameter name is already added
            foreach (var param in maParams.parameters)
                if (param.nameOrPrefix == parameterName)
                    return param;

            return new ParameterConfig();
        }

        public static void ReplaceParameterByName(
            PoseBuildContext ctx,
            ParameterConfig newParam
        )
        {
            var maParams = GetParamsFromContext(ctx);
            var parameterName = newParam.nameOrPrefix;
            
            // iterate over all the parameters and check if the parameter name is already added
            for (var i = 0; i < maParams.parameters.Count; i++)
            {
                Debug.Log(maParams.parameters[i].nameOrPrefix);
                if (maParams.parameters[i].nameOrPrefix == parameterName)
                {
                    maParams.parameters[i] = newParam;
                    return;
                }
            }

            Debug.LogError("[PoseEngine] Failed to find parameter to replace: " + parameterName);
        }

        private static ModularAvatarParameters GetParamsFromContext(
            PoseBuildContext ctx
        )
        {
            return ctx.poseEngineInstance.GetComponent<ModularAvatarParameters>();
        }
        
        public static void AddNewParameter(
            PoseBuildContext ctx,
            string parameterName,
            bool isSaved = false,
            ParameterSyncType syncType = ParameterSyncType.Float
        )
        {
            // ensure it doesn't already exist
            if (HasAddedParameter(ctx, parameterName))
                return;

            // create a new parameter config
            var maParams = GetParamsFromContext(ctx);
            var isLocalOnly = (syncType == ParameterSyncType.NotSynced && !isSaved);
            var newParam = new ParameterConfig
            {
                nameOrPrefix = parameterName,
                remapTo = parameterName,
                internalParameter = false,
                isPrefix = false,
                syncType = syncType,
                localOnly = isLocalOnly,
                defaultValue = 0.0f,
                saved = isSaved,
                hasExplicitDefaultValue = false
            };

            // add the new parameter to the list
            maParams.parameters.Add(newParam);
        }

        public static bool HasAddedParameter(
            PoseBuildContext ctx, string parameterName
        )
        {
            var maParams = ctx.poseEngineInstance.GetComponent<ModularAvatarParameters>();

            // iterate over all the parameters and check if the parameter name is already added
            foreach (var param in maParams.parameters)
                if (param.nameOrPrefix == parameterName)
                    return true;

            return false;
        }
    }
}