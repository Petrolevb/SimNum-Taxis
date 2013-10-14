using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SimNum_Taxis
{
	class AI
	{		
		/// <summary> Assigns client c to the taxi that will serve him the quickest among all availableTaxis. </summary>
		public static void AssignBestTaxiToCLient(Client c, List<Taxi> availableTaxis)
        {
            availableTaxis = availableTaxis.OrderBy((Func<Taxi, double>)(
            	(Taxi t1) => { return Util.Distance(t1.Position, c.Position); }))
            	.ToList();
        	
			// TODO add comparaison between angles, not distances.
			
			// If the closest taxi has a destination farther than the client c, we send the closest taxi
			// If not, we try  with 2nd closest taxi and, so on...
        	if(availableTaxis.Count > 0)
       			foreach(Taxi t in availableTaxis)
	       			if(Util.Distance(t.Position, c.Position) < Util.Distance(t.Position, t.Target))
	       			{
	       				t.Target = c.Position;
	       				break;
	       			}
       	}
		
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
    			
    			// Seeks the closest client and compare the distances between him and the current target.
    			// Possibly updates the target.
    												// TODO awaiting untargeted client
    			Client possibleNewClient = taxi.MyCity.getAwaitingClientClosestTo(taxi.Position);
    			if(possibleNewClient == null && distance == -1)
    				taxi.Target = new Point(-99999, -99999);
    			else
    			{
    				if( distance == -1 || Util.Distance(possibleNewClient.Position, taxi.Position) < distance)
    					taxi.Target = possibleNewClient.Position;
    			}
	    	}
    	}
		
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
    	
    	// TODO What if a client despawns ?
    	/// <summary> Deals with taxi's arrival to its target and updates the destination. </summary>
		/// <remarks> This method is only called when taxi has reached its target. </remarks>
    	public static void ManageArrival(Taxi taxi)
    	{
    		// Exit if taxi isn't arrived yet
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
				foreach(Client c in taxi.Clients)
					if(Util.Equivalent(taxi.Target, c.Destination))
					{
						taxi.MyCity.ClientPleased(c);
						taxi.Clients.Remove(c);
					}
			}
			AI.RefreshDestination(taxi);
    	}
	}
}
