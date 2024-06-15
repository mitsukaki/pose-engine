#region

using UnityEngine;
using UnityEditor;
using nadena.dev.modular_avatar.core;

#endregion

namespace com.mitsukaki.poseengine.editor
{
    public static class ParameterUtility
    {
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
            var maParams = ctx.poseEngineInstance.GetComponent<ModularAvatarParameters>();
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