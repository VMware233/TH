﻿#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VMFramework.OdinExtensions;

namespace VMFramework.Configuration
{
    [TypeValidation]
    public partial class DictionaryConfigs<TID, TConfig> : ITypeValidationProvider
    {
        IEnumerable<ValidationResult> ITypeValidationProvider.
            GetValidationResults(GUIContent label)
        {
            if (configs.Count == 0)
            {
                var labelName = label?.text;
                yield return new("缺少配置", ValidateType.Info);
            }
        }
    }
}
#endif