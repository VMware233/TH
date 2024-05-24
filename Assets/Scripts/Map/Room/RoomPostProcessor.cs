#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VMFramework.Core;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using TH.GameCore;
using UnityEditor;
using UnityEngine;
using VMFramework.Core.Editor;
using VMFramework.Core.JSON;
using EditorUtility = UnityEditor.EditorUtility;

namespace TH.Map
{
    public class RoomPostProcessor : AssetPostprocessor
    {
        private class RoomAssetMetaData
        {
            public int width;
            public int height;
            public Vector2Int dataStartPos;
            public List<string> types;
            public bool ignoreImport = false;
        }

        private static void OnPostprocessAllAssets(string[] importedAssets,
            string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
            {
                if (path.EndsWith(".room.xlsx"))
                {
                    var directoryPath = path.GetDirectoryPath();
                    var fileName = path.GetFileNameFromPath();

                    if (fileName.StartsWith("~$"))
                    {
                        continue;
                    }

                    var absolutePath = IOUtility.projectFolderPath.PathCombine(path);

                    using var stream = new FileStream(absolutePath, FileMode.Open);

                    stream.Position = 0;

                    XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);

                    var sheetsCount = xssWorkbook.NumberOfSheets;

                    for (int sheetIndex = 0; sheetIndex < sheetsCount; sheetIndex++)
                    {
                        var sheetName = xssWorkbook.GetSheetName(sheetIndex);

                        var roomAssetFileName = fileName.Replace(".room.xlsx", "") +
                                                $"-{sheetName}.asset";

                        var sheet = (XSSFSheet)xssWorkbook.GetSheetAt(sheetIndex);

                        var roomAssetMetaData = new RoomAssetMetaData();

                        for (int rowIndex = 0; rowIndex < sheet.LastRowNum; rowIndex++)
                        {
                            var row = sheet.GetRow(rowIndex);

                            if (row == null)
                            {
                                continue;
                            }

                            var cell = row.GetCell(0);

                            if (cell == null)
                            {
                                continue;
                            }

                            var rawData = cell.ToString();

                            if (rawData.IsNullOrEmpty())
                            {
                                continue;
                            }

                            var withoutComment = rawData.Split("//").FirstOrDefault();

                            if (withoutComment.IsNullOrEmpty())
                            {
                                continue;
                            }

                            var equationParts = withoutComment.Split('=');

                            if (equationParts.Length != 2)
                            {
                                continue;
                            }

                            var (dataName, dataValue) = (equationParts[0].Trim(),
                                equationParts[1].Trim());

                            var fieldInfo = roomAssetMetaData.GetType()
                                .GetFieldByName(dataName);

                            if (fieldInfo == null)
                            {
                                if (GameSetting.roomGeneralSetting.enableRoomAssetMetaFieldMissingWarning)
                                {
                                    Debug.LogWarning(
                                        $"Sheet:{sheetName} 的房间元数据中定义{dataName}字段不存在");
                                }

                                continue;
                            }

                            try
                            {
                                if (fieldInfo.FieldType.IsDerivedFrom<IList>(false))
                                {

                                }
                                else
                                {
                                    dataValue = "\"" + dataValue + "\"";
                                }

                                var propertyValue = JsonConvert.DeserializeObject(
                                    dataValue,
                                    fieldInfo.FieldType,
                                    JSONConverters.converters);

                                fieldInfo.SetValue(roomAssetMetaData, propertyValue);
                            }
                            catch (JsonReaderException e)
                            {
                                Debug.LogError(
                                    $"在读取第{rowIndex + 1}行，第1列，发生JSON解序列化错误，" +
                                    $"请注意格式，原始数据名称：{dataName}，原始数据内容:{dataValue}");
                                Debug.LogError(e);
                            }
                        }

                        if (roomAssetMetaData.ignoreImport)
                        {
                            Debug.Log($"Sheet:{sheetName} 的房间被忽略导入");
                            continue;
                        }

                        var width = roomAssetMetaData.width;
                        var height = roomAssetMetaData.height;
                        var dataStartPos = roomAssetMetaData.dataStartPos;

                        if (width == 0)
                        {
                            throw new ArgumentException($"Sheet:{sheetName} 的房间宽度没有定义");
                        }

                        if (height == 0)
                        {
                            throw new ArgumentException($"Sheet:{sheetName} 的房间高度没有定义");
                        }

                        if (dataStartPos.AnyNumberBelow(0))
                        {
                            throw new ArgumentException($"Sheet:{sheetName} 的房间数据起始位置不能小于0");
                        }

                        var roomRawData = new List<string>[width, height];

                        for (int rowIndex = dataStartPos.y;
                             rowIndex < dataStartPos.y + height;
                             rowIndex++)
                        {
                            var row = sheet.GetRow(rowIndex);

                            if (row == null)
                            {
                                continue;
                            }

                            for (int columnIndex = dataStartPos.x;
                                 columnIndex < dataStartPos.x + width;
                                 columnIndex++)
                            {
                                var cell = row.GetCell(columnIndex);

                                if (cell == null)
                                {
                                    continue;
                                }

                                var rawData = cell.ToString();

                                if (rawData.IsNullOrEmpty())
                                {
                                    continue;
                                }

                                roomRawData[columnIndex - dataStartPos.x,
                                    rowIndex - dataStartPos.y] = new List<string>()
                                {
                                    rawData
                                };
                            }
                        }

                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height / 2; y++)
                            {
                                (roomRawData[x, y], roomRawData[x, height - 1 - y]) =
                                    (roomRawData[x, height - 1 - y], roomRawData[x, y]);
                            }
                        }

                        var roomAssetPath = directoryPath.PathCombine(roomAssetFileName);

                        var roomAsset = roomAssetPath
                            .FindOrCreateScriptableObjectAtPath<RoomAsset>();

                        roomAsset.Load(new RoomRawInfo()
                        {
                            rawArray = roomRawData,
                            gameTypes = roomAssetMetaData.types
                        });

                        EditorUtility.SetDirty(roomAsset);

                        if (GameSetting.roomGeneralSetting.enableRoomImportSuccessLog)
                        {
                            Debug.Log($"房间{roomAsset.name}导入成功");
                        }
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

        }
    }
}
#endif