﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SimNum_Taxis
{
	class AI
	{	
    	#region Move taxi
    	/// <summary> Moves the taxi taxi. The shortest path is taken : straight line. </summary>
    	public static void move(Taxi taxi)
    	{
    		if(taxi.Target.X == -99999 && taxi.Target.Y == -99999)
    			return;
    		
    		for(int i=0; i < taxi.MyCity.RatioTime; i++)
    		{
    			double angle = Util.AngleForShortestPath(taxi.Position, taxi.Target);
    			taxi.Position = Util.MoveByPolar(taxi.Position, angle, taxi.Speed);
    		}
    	}
    	#endregion
	
    	#region Taxis assignement
		/// <summary> Assigns client c to the taxi that will serve him the quickest among all availableTaxis.
		/// 		  Is called whenever a client spawns. </summary>
		public static void AssignBestTaxiToClient(Client c, List<Taxi> availableTaxis)
        {
            availableTaxis = availableTaxis.OrderBy((Func<Taxi, double>)(
            	(Taxi t) => { return Util.Distance(t.Position, c.Position); }))
            	.ToList();

			// If the closest taxi has his current destination farther than the client c, we send the closest taxi
			// If not, we try  with 2nd closest taxi and so on...
        	if(availableTaxis.Count > 0)
       			foreach(Taxi t in availableTaxis)
					if(Util.Distance(t.Position, c.Position) < Util.Distance(t.Position, t.Target))
					{
        				AssignTaxiToClient(t, c);
						break;
					}	
       	}
		
		public static void AssignTaxiToClient(Taxi taxi, Client c)
		{
			// If the taxi was already targeting a client, this old client is no longer targeted.
    		if(taxi.TargetedClient != null)
    			taxi.TargetedClient.Targeted = false;
    		// And we now target the new client.
    		taxi.Target = c.Position;
			c.Targeted = true;
			taxi.TargetedClient = c;
		}
		#endregion
		
		#region Destination calculations
		/// <summary> Tells the taxi taxi where to go according to its clients. </summary>
    	public static void RefreshDestination(Taxi taxi)
    	{
    		// If the taxi has 2 clients, it delivers first the closest one
    		if(taxi.Clients.Count == 2)
    		{
    			if(Util.Distance(taxi.Position, taxi.Clients[0].Destination) < Util.Distance(taxi.Position, taxi.Clients[1].Destination))
    				taxi.Target = taxi.Clients[0].Destination;
    			else
    				taxi.Target = taxi.Clients[1].Destination;
    		}
    		
    		// Otherwise, it goes to the only clients' destination OR to the closest unprocessed client
    		else
    		{
    			double distance = -1;
    			if(taxi.Clients.Count == 1)
    			{
    				taxi.Target = taxi.Clients[0].Destination;
    				distance = Util.Distance(taxi.Target, taxi.Position);
    			}

    			// Seeks the closest client and compares the distances between him and the current target.
    			// Possibly updates the target.
    			Client possibleNewClient = taxi.MyCity.getAwaitingUnprocessedClientClosestTo(taxi.Position);
    			if(possibleNewClient == null)
    			{
    				if(distance == -1)
    					taxi.Target = new Point(-99999, -99999);
    			}
    			else if(distance == -1 || Util.Distance(possibleNewClient.Position, taxi.Position) < distance)
    				AssignTaxiToClient(taxi, possibleNewClient);
	    	}
    	}
    	
    	/// <summary> Deals with taxi's arrival to its target and updates the destination. </summary>
		/// <remarks> This method is only called when taxi has reached its target. </remarks>
    	public static void ManageArrival(Taxi taxi)
    	{
    		// Exits if taxi isn't arrived yet. This is a safety line.
			if(!Util.Equivalent(taxi.Target, taxi.Position))
				return;

			// Did the taxi pick up a client ?
			bool targetWasAClient = false;
			if(taxi.Clients.Count < 2)
			{
				Client c = taxi.MyCity.getClientWaitingAtPosition(taxi.Position);
								
				if(c != null)
				{
					taxi.Clients.Add(c);
					c.MyTaxi = taxi;
					taxi.MyCity.ClientPickedUp();
					targetWasAClient = true;					
				}
			}
			
			// If not, the taxi reached a client destination => Success
			if(!targetWasAClient)
			{
				for(int i = 0; i < taxi.Clients.Count; i++)
        		{
					if(Util.Equivalent(taxi.Target, taxi.Clients[i].Destination))
					{
						taxi.MyCity.ClientPleased(taxi.Clients[i]);
						taxi.Clients.Remove(taxi.Clients[i]);
					}
        		}
			}
			AI.RefreshDestination(taxi);
    	}
    	#endregion
	}
}
