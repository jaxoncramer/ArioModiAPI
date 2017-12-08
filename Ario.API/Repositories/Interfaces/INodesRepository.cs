/** 
 * Filename: INodesRepository.cs
 * Description: Contains method definitions for NodesRepository.cs
 *              for validation and sanity checks. Interfaces are used in 
 *              controllers, not the repos themselves.
 * 
 * Author: Jaxon Cramer, November 2017
 *
 * Copyright: The following source code is the sole property of Ario, Inc., all
 *              rights reserved. This code should not be copied or distributed
 *              without the express written consent of Ario, Inc.
 * 
 **/


using System;
using System.Collections.Generic;
using Ario.API.Models.DisplayModels;
using Ario.API.Models.Objects;

namespace Ario.API.Repositories
{
    public interface INodesRepository
	{
        void Add(NodesDisplay item, out int status);
        IEnumerable<NodesDisplay> GetSelect(NodesDisplay item);
        IEnumerable<NodesDisplay> GetByBusiness(int id, NodesDisplay disp);
        IEnumerable<NodesDisplay> GetNodesOnTeam(int teamID);
        IEnumerable<NodesDisplay> GetChildren(int busID, int nodeID, out int status);
        NodesDisplay Find(int id);
		bool Remove(int id, out int status);
        void Update(int id, NodesDisplay newNode, out int status);
        bool NodeExists(int id);
        void AddComponent(Int64 id, NodeComponentData item, out int status);
        bool DropComponent(int nodeID, int compID, out int status);
	}
}
