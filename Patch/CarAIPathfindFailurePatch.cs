﻿using ColossalFramework;
using HarmonyLib;
using MoreEffectiveTransfer.Util;
using System.Reflection;

namespace MoreEffectiveTransfer.Patch
{
    [HarmonyPatch]
    public class CarAIPathfindFailurePatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(CarAI).GetMethod("PathfindFailure", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public static void Postfix(ushort vehicleID, ref Vehicle data)
        {
            RecordFailedBuilding(vehicleID, ref data);
        }

        public static bool NeedCheckPathFind(TransferManager.TransferReason material)
        {
            switch (material)
            {
                case TransferManager.TransferReason.Oil:
                case TransferManager.TransferReason.Ore:
                case TransferManager.TransferReason.Coal:
                case TransferManager.TransferReason.Petrol:
                case TransferManager.TransferReason.Food:
                case TransferManager.TransferReason.Grain:
                case TransferManager.TransferReason.Lumber:
                case TransferManager.TransferReason.Logs:
                case TransferManager.TransferReason.Goods:
                case TransferManager.TransferReason.LuxuryProducts:
                case TransferManager.TransferReason.AnimalProducts:
                case TransferManager.TransferReason.Flours:
                case TransferManager.TransferReason.Petroleum:
                case TransferManager.TransferReason.Plastics:
                case TransferManager.TransferReason.Metals:
                case TransferManager.TransferReason.Glass:
                case TransferManager.TransferReason.PlanedTimber:
                case TransferManager.TransferReason.Paper:
                case TransferManager.TransferReason.Fire:
                case TransferManager.TransferReason.Garbage:
                case TransferManager.TransferReason.GarbageMove:
                case TransferManager.TransferReason.Crime:
                case TransferManager.TransferReason.CriminalMove:
                case TransferManager.TransferReason.Dead:
                case TransferManager.TransferReason.DeadMove:
                case TransferManager.TransferReason.Snow:
                case TransferManager.TransferReason.SnowMove:
                case TransferManager.TransferReason.RoadMaintenance:
                case TransferManager.TransferReason.ParkMaintenance:
                case TransferManager.TransferReason.Taxi:
                    return true;
                default: return false;
            }
        }

        public static void RecordFailedBuilding(ushort vehicleID, ref Vehicle data)
        {
            if (MoreEffectiveTransfer.fixUnRouteTransfer)
            {
                if (NeedCheckPathFind((TransferManager.TransferReason)data.m_transferType))
                {
                    if (data.m_targetBuilding != 0)
                    {
                        if (data.m_sourceBuilding != 0)
                        {
                            bool alreadyHaveFailedBuilding = false;
                            for (int j = 0; j < MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding]; j++)
                            {
                                if (MainDataStore.canNotConnectedBuildingID[data.m_targetBuilding, j] == data.m_sourceBuilding)
                                {
                                    alreadyHaveFailedBuilding = true;
                                    break;
                                }
                            }

                            if (!alreadyHaveFailedBuilding)
                            {
                                if (MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding] < 255)
                                {
                                    MainDataStore.canNotConnectedBuildingID[data.m_targetBuilding, MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding]] = data.m_sourceBuilding;
                                    MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding]++;
                                    if (MoreEffectiveTransfer.debugMode)
                                    {
                                        var building1 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_targetBuilding];
                                        var building2 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_sourceBuilding];
                                        DebugLog.LogToFileOnly("Connect failed begin, count = " + MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding].ToString());
                                        DebugLog.LogToFileOnly("DebugInfo: m_targetBuilding m_class is " + building1.Info.m_class.ToString());
                                        DebugLog.LogToFileOnly("DebugInfo: m_targetBuilding name is " + building1.Info.name.ToString());
                                        DebugLog.LogToFileOnly("DebugInfo: m_targetBuilding id is " + data.m_targetBuilding.ToString());
                                        DebugLog.LogToFileOnly("DebugInfo: m_sourceBuilding m_class is " + building2.Info.m_class.ToString());
                                        DebugLog.LogToFileOnly("DebugInfo: m_sourceBuilding name is " + building2.Info.name.ToString());
                                        DebugLog.LogToFileOnly("DebugInfo: m_sourceBuilding id is " + data.m_sourceBuilding.ToString());
                                        DebugLog.LogToFileOnly("Connect failed end");
                                    }
                                }
                                else
                                {
                                    if (MoreEffectiveTransfer.debugMode)
                                    {
                                        DebugLog.LogToFileOnly("Error: Max canNotConnectedBuildingIDCount 255 reached, Please check your roadnetwork");
                                        var building1 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_targetBuilding];
                                        DebugLog.LogToFileOnly("DebugInfo: building m_class is " + building1.Info.m_class.ToString());
                                        DebugLog.LogToFileOnly("DebugInfo: building name is " + building1.Info.name.ToString());
                                        DebugLog.LogToFileOnly("DebugInfo: building id is " + data.m_targetBuilding.ToString());
                                        DebugLog.LogToFileOnly("Error: Max canNotConnectedBuildingIDCount 255 reached, End");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
