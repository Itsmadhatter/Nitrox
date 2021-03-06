﻿using System;
using NitroxModel.Packets;
using NitroxServer.Communication.Packets.Processors.Abstract;
using NitroxModel.Tcp;
using NitroxServer.GameLogic;

namespace NitroxServer.Communication.Packets.Processors
{
    public class ClaimPlayerSlotReservationProcessor : UnauthenticatedPacketProcessor<ClaimPlayerSlotReservation>
    {
        private readonly TimeKeeper timeKeeper;
        private readonly EscapePodManager escapePodManager;
        private readonly PlayerManager playerManager;

        public ClaimPlayerSlotReservationProcessor(TimeKeeper timeKeeper, EscapePodManager escapePodManager, PlayerManager playerManager)
        {
            this.timeKeeper = timeKeeper;
            this.escapePodManager = escapePodManager;
            this.playerManager = playerManager;
        }

        public override void Process(ClaimPlayerSlotReservation packet, Connection connection)
        {
            Player player = playerManager.ClaimPlayerSlotReservation(connection, packet.ReservationKey, packet.CorrelationId);
            player.SendPacket(new TimeChange(timeKeeper.GetCurrentTime()));

            escapePodManager.AssignPlayerToEscapePod(player.Id);

            BroadcastEscapePods broadcastEscapePods = new BroadcastEscapePods(escapePodManager.GetEscapePods());
            playerManager.SendPacketToAllPlayers(broadcastEscapePods);
        }
    }
}
