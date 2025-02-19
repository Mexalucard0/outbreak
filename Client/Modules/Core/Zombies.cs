﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Outbreak
{
    class Zombie : BaseScript
    {
        Config Config = new Config();
        private string PlayerGroup = "PLAYER";
        private string ZombieGroup = "ZOMBIE";
        public Zombie()
        {
            uint GroupHandle = 0;
            AddRelationshipGroup(ZombieGroup, ref GroupHandle);
            SetRelationshipBetweenGroups(0, (uint)GetHashKey(ZombieGroup), (uint)GetHashKey(PlayerGroup));
            SetRelationshipBetweenGroups(5, (uint)GetHashKey(PlayerGroup), (uint)GetHashKey(ZombieGroup));

            Tick += OnTick;
        }

        private async Task OnTick()
        {
            int PedHandle = -1;
            bool success;
            int Handle = FindFirstPed(ref PedHandle);

            do
            {
                await Delay(10);

                if (IsPedHuman(PedHandle) & !IsPedAPlayer(PedHandle) & !IsPedDeadOrDying(PedHandle, true))
                {
                    if (GetRelationshipBetweenPeds(PedHandle, PlayerPedId()) != 0)
                    {
                        ClearPedTasks(PedHandle);
                        TaskWanderStandard(PedHandle, 10.0f, 10);
                        SetPedRelationshipGroupHash(PedHandle, (uint)GetHashKey(ZombieGroup));
                        ApplyPedDamagePack(PedHandle, "BigHitByVehicle", 0.0f, 9.0f);
                        SetPedConfigFlag(PedHandle, 100, false);
                        SetEntityHealth(PedHandle, 500);

                        if (IsPedInAnyVehicle(PedHandle, false))
                        {
                            DeletePed(ref PedHandle);
                        }
                    }

                    Vector3 PlayerCoords = GetEntityCoords(PlayerPedId(), false);
                    Vector3 PedsCoords = GetEntityCoords(PedHandle, false);
                    var Distance = GetDistanceBetweenCoords(PlayerCoords.X, PlayerCoords.Y, PlayerCoords.Z, PedsCoords.X, PedsCoords.Y, PedsCoords.Z, true);

                    if (Distance <= Config.DistanceZombieTargetToPlayer & !GetPedConfigFlag(PedHandle, 100, false))
                    {
                        SetPedConfigFlag(PedHandle, 100, true);
                        ClearPedTasks(PedHandle);
                        if (Config.ZombieCanRun)
                        {
                            TaskGoToEntity(PedHandle, PlayerPedId(), -1, 0.0f, 2.0f, 1073741824, 0);
                        }
                        else { TaskGoToEntity(PedHandle, PlayerPedId(), -1, 0.0f, 1.0f, 1073741824, 0); }
                    }

                    if (Distance <= 1.3f)
                    {
                        Debug.WriteLine($"{GetEntityHealth(PlayerPedId())}");
                        
                        if (!IsPedRagdoll(PedHandle) && !IsPedGettingUp(PedHandle))
                        {
                            if (GetEntityHealth(PlayerPedId()) == 0)
                            {
                                ClearPedTasks(PedHandle);
                                TaskWanderStandard(PedHandle, 10.0f, 10);
                            }
                            else
                            {
                                RequestAnimSet("melee@unarmed@streamed_core_fps");
                                while (!HasAnimSetLoaded("melee@unarmed@streamed_core_fps"))
                                {
                                    await Delay(1);
                                }

                                TaskPlayAnim(PedHandle, "melee@unarmed@streamed_core_fps", "ground_attack_0_psycho", 8.0f, 1.0f, -1, 48, 0.001f, false, false, false);
                                ApplyDamageToPed(PlayerPedId(), Config.ZombieDamage, false);
                            }
                        }
                    }

                    if (!NetworkGetEntityIsNetworked(PedHandle))
                    {
                        DeletePed(ref PedHandle);
                    }

                    ZombiePedAttributes(PedHandle);

                    RequestAnimSet("move_m@drunk@verydrunk");
                    while (!HasAnimSetLoaded("move_m@drunk@verydrunk"))
                    {
                        await Delay(1);
                    }
                    SetPedMovementClipset(PedHandle, "move_m@drunk@verydrunk", 1.0f);
                    
                    if (Config.Debug)
                    {
                        World.DrawMarker(MarkerType.VerticalCylinder, PedsCoords + new Vector3(0, 0, -1), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1f, 1f, 2f), Color.FromArgb(255, 255, 255, 255));
                    }

                }

                success = FindNextPed(Handle, ref PedHandle);
            } while (success);

            EndFindPed(Handle);

            await Delay(500);
        }
        
        private void ZombiePedAttributes(int ZombiePed)
        {
            SetPedRagdollBlockingFlags(ZombiePed, 1); //Works
            SetPedCanRagdollFromPlayerImpact(ZombiePed, false); // Works
            SetPedSuffersCriticalHits(ZombiePed, false); //Works
            SetPedEnableWeaponBlocking(ZombiePed, true); //Works
            DisablePedPainAudio(ZombiePed, true); //Works
            StopPedSpeaking(ZombiePed, true); // Works
            SetPedDiesWhenInjured(ZombiePed, false); // Works
            StopPedRingtone(ZombiePed); //Maybe dont works
            //SetPedMaxHealth(ZombiePed, 1000);
            //SetPedMute(ZombiePed); // test
            //ClearPedTasksImmediately(ZombiePed);
            //ClearPedSecondaryTask(ZombiePed);
            //ClearPedTasks(ZombiePed);
            SetPedIsDrunk(ZombiePed, true); //Maybe
            SetPedConfigFlag(ZombiePed, 166, false); // Maybe dont works
            SetPedConfigFlag(ZombiePed, 170, false); // Maybe dont works
            //TaskSetBlockingOfNonTemporaryEvents(ZombiePed, true); // More tests /Maybe dont works
            SetBlockingOfNonTemporaryEvents(ZombiePed, true); // Works
            SetPedCanEvasiveDive(ZombiePed, false); // Works
            RemoveAllPedWeapons(ZombiePed, true); // Works
        }

    }
}
