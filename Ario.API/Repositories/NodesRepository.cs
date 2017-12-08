/** 
 * Filename: NodesRepository.cs
 * Description: Contains the source logic for API endpoints for managing and 
 *              editing nodes registered with the Ario platform.
 * 
 * Author: Jaxon Cramer, November 2017
 *
 * Copyright: The following source code is the sole property of Ario, Inc., all
 *              rights reserved. This code should not be copied or distributed
 *              without the express written consent of Ario, Inc.
 * 
 **/

using System.Collections.Generic;
using System.Linq;
using Ario.API.Models;
using Ario.API.Contexts;
using Ario.API.Models.DisplayModels;
using System;
using System.Reflection;
using Ario.API.Attributes;
using Ario.API.Models.Objects;

namespace Ario.API.Repositories
{

    /// <summary>
    /// Contains the source logic for API endpoints for managing and 
    /// editing nodes registered with the Ario platform.
    /// </summary>
    /// <seealso cref="Controllers.NodesController"/>
    /// <seealso cref="Controllers.BusinessesController"/>
	public class NodesRepository : INodesRepository
	{
        public enum NodeTypes : Int64 {
            Label = 1,
            Image = 2,
            Video = 3,
            Audio = 4,
            Document = 5
        }

		NodesContext _nodeContext;
        NodeComponentsContext _compContext;
        LabelsContext _labelContext;
        QRAnchorComponentsContext _qrAnchorContext;
        NodeTeamJoinContext _nodeTeamContext;
        PDFComponentsContext _pdfContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Ario.API.Repositories.NodesRepository"/> class.
        /// </summary>
        /// <param name="nodeContext">Nodes table context from database.</param>
        /// <param name="compContext">NodeComponents table context from database.</param>
        /// <param name="labelContext">Labels table context from database.</param>
        /// <param name="qrAnchorContext">QRAnchorComponents table context from database.</param>
        public NodesRepository(NodesContext nodeContext, 
                               NodeComponentsContext compContext, 
                               LabelsContext labelContext, 
                               QRAnchorComponentsContext qrAnchorContext, 
                               NodeTeamJoinContext nodeTeamContext,
                               PDFComponentsContext pdfContext)
		{
            _nodeContext = nodeContext;
            _compContext = compContext;
            _labelContext = labelContext;
            _qrAnchorContext = qrAnchorContext;
            _nodeTeamContext = nodeTeamContext;
            _pdfContext = pdfContext;
		}

        /// <summary>
        /// Gets a list of nodes filtered by node object parameter
        /// </summary>
        /// <returns>A list of filtered nodes.</returns>
        /// <param name="item">Node object contained in node database table with desired fields.</param>
        public IEnumerable<NodesDisplay> GetSelect(NodesDisplay item)
		{

			List<NodesDisplay> displayList = new List<NodesDisplay>();

			if (item != null)
			{
				List<Nodes> nodeList = _nodeContext.Nodes.ToList();

				foreach (Nodes entry in nodeList)
                {
                    bool match = true;
                    NodesDisplay disp = CreateDisplay(entry);

                    Type type = disp.GetType();
                    foreach (PropertyInfo propertyInfo in type.GetProperties())
                    {
                        if (propertyInfo.CanRead)
                        {
                            if (propertyInfo.GetValue(item) != null && !propertyInfo.GetValue(item).ToString().Equals("0"))
                            {
                                if (Attribute.IsDefined(propertyInfo, typeof(BlackListed)))
                                {
                                    match = false;
                                    break;
                                }
                                if (propertyInfo.GetValue(disp) == null)
                                {
                                    match = false;
                                    break;
                                }
                                if (!propertyInfo.GetValue(disp).Equals(propertyInfo.GetValue(item)))
                                {
                                    match = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (match) { displayList.Add(disp); }

                }
			}

			return displayList;
		}

        /// <summary>
        /// Gets all of the nodes assigned to the specified business 
        /// and filtered by node object parameter.
        /// </summary>
        /// <returns>List of nodes assigned to the business.</returns>
        /// <param name="id">Business Identifier.</param>
        /// <param name="disp">Node object contained in node database table with desired fields.</param>
        public IEnumerable<NodesDisplay> GetByBusiness(int id, NodesDisplay disp) {
            disp.businessID = id;
            IEnumerable<NodesDisplay> displayList = GetSelect(disp);
            return displayList;
        }

        /// <summary>
        /// Gets the children of a specified node, unless the node is not
        /// a part of the specified business.
        /// </summary>
        /// <returns>A list of children to the specified node, or null if permission denied.</returns>
        /// <param name="busID">Business identifier.</param>
        /// <param name="nodeID">Node identifier.</param>
        /// <param name="status">Status code pointer.</param>
        public IEnumerable<NodesDisplay> GetChildren(int busID, int nodeID, out int status) {
            Nodes parent = _nodeContext.Nodes.SingleOrDefault(n => n.id == nodeID);

            if(parent.businessID != busID) {
                status = 400;
                return null;
            }

            List<NodesDisplay> displayList = new List<NodesDisplay>();

            List<Nodes> nodeList = _nodeContext.Nodes.Where(l => l.parentID == parent.id).ToList();
            foreach (Nodes node in nodeList)
            {
                displayList.Add(CreateDisplay(node));
            }

            status = 200;
            return displayList;
        }

        /// <summary>
        /// Gets all nodes visible to a team.
        /// </summary>
        /// <returns>The nodes on team.</returns>
        /// <param name="teamID">Team identifier.</param>
        public IEnumerable<NodesDisplay> GetNodesOnTeam(int teamID) {
            List<NodesDisplay> dispList = new List<NodesDisplay>();

            List<NodeTeamJoin> joinList = _nodeTeamContext.NodeTeamJoin.Where(j => j.teamID == teamID).ToList();
            foreach(NodeTeamJoin joinEntry in joinList) {
                Nodes node = _nodeContext.Nodes.SingleOrDefault(n => n.id == joinEntry.nodeID);
                if (node != null) {
                    NodesDisplay disp = CreateDisplay(node);
                    dispList.Add(disp);
                }
            }

            return dispList;
        }

        /// <summary>
        /// Checks to see of the specified node exists
        /// </summary>
        /// <returns><c>true</c>, if the node exists in the Ario database, <c>false</c> otherwise.</returns>
        /// <param name="id">Node Identifier.</param>
        public bool NodeExists(int id) 
        {
            Nodes node = _nodeContext.Nodes.Where(n => n.id == id).SingleOrDefault();
            if (node == null) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the node information that corresponds to the 
        /// specified node ID
        /// </summary>
        /// <returns>The node display mask, if found, or null if not.</returns>
        /// <param name="id">Node Identifier.</param>
        public NodesDisplay Find(int id)
        {
            Nodes node = _nodeContext.Nodes.Where(n => n.id == id).SingleOrDefault();
            if (node != null)
            {
                return CreateDisplay(node);
            }
            return null;
        }

        /// <summary>
        /// Create a node display mask (which includes all component information) 
        /// from a node database object
        /// </summary>
        /// <returns>The node display mask .</returns>
        /// <param name="node">Node database object.</param>
        private NodesDisplay CreateDisplay(Nodes node) {
            
            NodesDisplay disp = new NodesDisplay(node);

            List<NodeTeamJoin> teamJoinList = _nodeTeamContext.NodeTeamJoin.Where(j => j.nodeID == node.id).ToList();
            foreach (NodeTeamJoin joinEntry in teamJoinList) {
                disp.teams.Add(joinEntry.teamID);
            }

            List<NodeComponents> compList = _compContext.NodeComponents.Where(c => c.nodeID == node.id).ToList();
            foreach (NodeComponents comp in compList)
            {
                switch (comp.type)
                {
                    case "Label":
                    case "label":
                        Labels label = _labelContext.Labels.Where(l => l.nodeComponentID == comp.id).SingleOrDefault();

                        if (label != null)
                        {
                            LabelNodeComponentData labelData = new LabelNodeComponentData();
                            labelData.nodeComponentID = label.nodeComponentID;
                            labelData.nodeID = comp.nodeID;
                            labelData.type = comp.type;
                            labelData.description = label.text;
                            labelData.color = label.style;
                            labelData.screenshot = label.screenshot;

                            disp.components.Add(labelData);
                        }
                        break;

                    case "QRAnchor":
                    case "qranchor":
                    case "QRanchor":
                        QRAnchorComponents anchor = _qrAnchorContext.QRAnchorComponents.Where(q => q.nodeComponentID == comp.id).SingleOrDefault();

                        if (anchor != null)
                        {
                            QRAnchorNodeComponentData anchorData = new QRAnchorNodeComponentData();
                            anchorData.nodeComponentID = anchor.nodeComponentID;
                            anchorData.nodeID = comp.nodeID;
                            anchorData.type = comp.type;
                            anchorData.qrAnchorID = anchor.qrAnchorID;

                            disp.components.Add(anchorData);
                        }
                        break;

                    case "PDF":
                    case "pdf":
                        PDFComponents pdf = _pdfContext.PDFComponents.Where(l => l.nodeComponentID == comp.id).SingleOrDefault();

                        if (pdf != null)
                        {
                            PDFNodeComponentData pdfData = new PDFNodeComponentData();
                            pdfData.nodeComponentID = pdf.nodeComponentID;
                            pdfData.nodeID = comp.nodeID;
                            pdfData.type = comp.type;
                            pdfData.pdfLink = pdf.pdfLink;
                            pdfData.title = pdf.title;
                            pdfData.description = pdf.description;

                            disp.components.Add(pdfData);
                        }
                        break;
                }
            }

            return disp;
        }

        //TODO: Implement check that user is only allowed to add 
        //          node in their own business
        /// <summary>
        /// Add a node (with components) to the Ario database
        /// </summary>
        /// <param name="item">Node display mask containing node and component information.</param>
        public void Add(NodesDisplay item, out int status)
        {
            Nodes node = new Nodes(item);
            Nodes tempNode = _nodeContext.Nodes.OrderByDescending(n => n.id).FirstOrDefault();
            if(tempNode != null) {
                node.id = tempNode.id + 1;
            } else {
                node.id = 1;
            }
            item.id = node.id;

            _nodeContext.Nodes.Add(node);
            try
            {
                _nodeContext.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                Console.WriteLine(e);
                status = 507;
                return;
            }

            if (item.teams != null) {
                foreach (Int64 teamID in item.teams) {
                    CreateTeamJoin(node.id, teamID, out status);
                    if (status != 200) {
                        return;
                    }
                }
            }

            if(item.components != null) {
                
                foreach (NodeComponentData data in item.components)
                {
                    data.nodeID = node.id;
                    Int64 id = CreateComp(data, out status);
                    if (status != 200) {
                        return;
                    }
                    CreateCompType(id, data, out status);
                    if (status != 200) {
                        return;
                    }
                }
            }
            status = 201;
            return;
        }

        /// <summary>
        /// Creates an entry in the Node-Team Join table to set node visibility
        /// </summary>
        /// <param name="nodeID">Node identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        private void CreateTeamJoin(Int64 nodeID, Int64 teamID, out int status) {
            NodeTeamJoin joinEntry = new NodeTeamJoin();
            joinEntry.nodeID = nodeID;
            joinEntry.teamID = teamID;

            NodeTeamJoin tempJoin = _nodeTeamContext.NodeTeamJoin.OrderByDescending(n => n.id).FirstOrDefault();
            if(tempJoin != null) {
                joinEntry.id = tempJoin.id + 1;
            }
            else {
                joinEntry.id = 1;
            }

            _nodeTeamContext.NodeTeamJoin.Add(joinEntry);
            try
            {
                _nodeTeamContext.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                Console.WriteLine(e);
                status = 507;
                return;
            }
            status = 200;
            return;
        }

        //TODO: Implement check that user is only allowed to add component to 
        //          node in their own business
        /// <summary>
        /// Public method to add a new component to an existing node,
        /// specified by the id parameter
        /// </summary>
        /// <param name="id">Node Identifier.</param>
        /// <param name="data">Node Component information to be added.</param>
        public void AddComponent(Int64 id, NodeComponentData data, out int status)
        {
            data.nodeID = id;
            Int64 compID = CreateComp(data, out status);
            if (status != 200) {
                return;
            }
            CreateCompType(compID, data, out status);
            if (status != 200) {
                return;
            }
        }

        /// <summary>
        /// Creates an entry in the node components Ario database table with
        /// the information provided in the node component data object.
        /// </summary>
        /// <returns>The newly created component identifier.</returns>
        /// <param name="data">Node component data object.</param>
        private Int64 CreateComp(NodeComponentData data, out int status)
        {
            NodeComponents comp = new NodeComponents(data);
            NodeComponents tempComp = _compContext.NodeComponents.OrderByDescending(c => c.id).FirstOrDefault();
            if (tempComp != null)
            {
                comp.id = tempComp.id + 1;
            }
            else
            {
                comp.id = 1;
            }

            _compContext.NodeComponents.Add(comp);
            try
            {
                _compContext.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                Console.WriteLine(e);
                status = 507;
                return 0;
            }
            status = 200;
            return comp.id;
        }

        /// <summary>
        /// Creates the specific component type that corresponds to the 
        /// data.type passed in the node component data object
        /// </summary>
        /// <param name="id">Node Identifier.</param>
        /// <param name="data">Node component data object.</param>
        private void CreateCompType(Int64 id, NodeComponentData data, out int status) {
            
            switch (data.type)
            {
                //Create a label component
                case "Label":
                case "label":
                    Labels label = new Labels();
                    label.nodeComponentID = id;
                    label.text = ((LabelNodeComponentData)data).description;
                    label.style = ((LabelNodeComponentData)data).color;
                    label.screenshot = ((LabelNodeComponentData)data).screenshot;

                    Labels tempLabel = _labelContext.Labels.OrderByDescending(l => l.id).FirstOrDefault();
                    if (tempLabel != null)
                    {
                        label.id = tempLabel.id + 1;
                    }
                    else
                    {
                        label.id = 1;
                    }

                    _labelContext.Labels.Add(label);
                    try
                    {
                        _labelContext.SaveChanges();
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                    {
                        Console.WriteLine(e);
                        status = 507;
                        return;
                    }
                    break;
                //create a QR Anchor component
                case "QRAnchor":
                case "qranchor":
                case "QRanchor":
                    QRAnchorComponents anchor = new QRAnchorComponents();
                    anchor.nodeComponentID = id;
                    anchor.qrAnchorID = ((QRAnchorNodeComponentData)data).qrAnchorID;

                    QRAnchorComponents tempAnchor = _qrAnchorContext.QRAnchorComponents.OrderByDescending(q => q.id).FirstOrDefault();
                    if (tempAnchor != null)
                    {
                        anchor.id = tempAnchor.id + 1;
                    }
                    else
                    {
                        anchor.id = 1;
                    }

                    _qrAnchorContext.QRAnchorComponents.Add(anchor);
                    try
                    {
                        _qrAnchorContext.SaveChanges();
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                    {
                        Console.WriteLine(e);
                        status = 507;
                        return;
                    }                    
                    break;
                    //Create a label component
                case "PDF":
                case "pdf":
                    PDFComponents pdf = new PDFComponents();
                    pdf.nodeComponentID = id;
                    pdf.pdfLink = ((PDFNodeComponentData)data).pdfLink;
                    pdf.title = ((PDFNodeComponentData)data).title;
                    pdf.description = ((PDFNodeComponentData)data).description;

                    PDFComponents tempPDF = _pdfContext.PDFComponents.OrderByDescending(l => l.id).FirstOrDefault();
                    if (tempPDF != null)
                    {
                        pdf.id = tempPDF.id + 1;
                    }
                    else
                    {
                        pdf.id = 1;
                    }

                    _pdfContext.PDFComponents.Add(pdf);
                    try
                    {
                        _pdfContext.SaveChanges();
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                    {
                        Console.WriteLine(e);
                        status = 507;
                        return;
                    }
                    break;
            }
            status = 200;
            return;
        }

        /// <summary>
        /// Update node information with the non-null fields contained in the 
        /// given node display object. 
        /// </summary>
        /// <remarks>Does not handle adding or deleting components
        /// that do not already exist in the node.</remarks>
        /// <param name="id">Node Identifier.</param>
        /// <param name="newNode">Display mask with the node information being updated.</param>
        public void Update(int id, NodesDisplay newNode, out int status)
		{
			Nodes nodeToUpdate = _nodeContext.Nodes.SingleOrDefault(r => r.id == id);
			if (nodeToUpdate != null)
			{
                if (newNode.parentID != 0) {
                    nodeToUpdate.parentID = newNode.parentID;
                }
                if (newNode.businessID != 0)
                {
                    nodeToUpdate.businessID = newNode.businessID;
                }
                if (newNode.nodeName != null) {
                    nodeToUpdate.nodeName = newNode.nodeName;
                }
                if (newNode.position != null) {
                    nodeToUpdate.xPosition = newNode.position.x;
                    nodeToUpdate.yPosition = newNode.position.y;
                    nodeToUpdate.zPosition = newNode.position.z;
                }
                try {
                    _nodeContext.SaveChanges();
                } catch(Microsoft.EntityFrameworkCore.DbUpdateException e) {
                    Console.WriteLine(e);
                    status = 507;
                    return;
                }
			}

            if (newNode.teams != null) {
                List<NodeTeamJoin> teamJoinList = _nodeTeamContext.NodeTeamJoin.Where(j => j.nodeID == id).ToList();
                List<Int64> tempTeamArray = new List<Int64>();

                foreach(NodeTeamJoin joinEntry in teamJoinList) {
                    if (!newNode.teams.Contains(joinEntry.teamID)) {
                        _nodeTeamContext.NodeTeamJoin.Remove(joinEntry);
                        try
                        {
                            _nodeTeamContext.SaveChanges();
                        }
                        catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                        {
                            Console.WriteLine(e);
                            status = 507;
                            return;
                        }                    
                    }
                    else {
                        tempTeamArray.Add(joinEntry.teamID);
                    }
                }

                foreach(Int64 teamID in newNode.teams) {
                    if (!tempTeamArray.Contains(teamID)) {
                        CreateTeamJoin(id, teamID, out status);
                        if (status != 200) {
                            return;
                        }
                    }
                }
            }

            if (newNode.components != null)
            {
                foreach(NodeComponentData comp in newNode.components) {
                    if (comp.nodeComponentID != 0)
                    {
                        switch (comp.type)
                        {
                            case "Label":
                            case "label":
                                Labels labelToUpdate = _labelContext.Labels.Where(l => l.nodeComponentID == comp.nodeComponentID).SingleOrDefault();
                                if (labelToUpdate != null)
                                {
                                    if (((LabelNodeComponentData)comp).color != null)
                                    {
                                        labelToUpdate.style = ((LabelNodeComponentData)comp).color;
                                    }
                                    if (((LabelNodeComponentData)comp).description != null)
                                    {
                                        labelToUpdate.text = ((LabelNodeComponentData)comp).description;
                                    }
                                    if (((LabelNodeComponentData)comp).screenshot != null)
                                    {
                                        labelToUpdate.screenshot = ((LabelNodeComponentData)comp).screenshot;
                                    }
                                    try
                                    {
                                        _labelContext.SaveChanges();
                                    }
                                    catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                                    {
                                        Console.WriteLine(e);
                                        status = 507;
                                        return;
                                    }                                
                                }
                                break;
                            case "QRAnchor":
                            case "qranchor":
                            case "QRanchor":
                                QRAnchorComponents qrToUpdate = _qrAnchorContext.QRAnchorComponents.Where(q => q.nodeComponentID == comp.nodeComponentID).SingleOrDefault();
                                if (qrToUpdate != null)
                                {
                                    if(((QRAnchorNodeComponentData)comp).qrAnchorID != 0) {
                                        qrToUpdate.qrAnchorID = ((QRAnchorNodeComponentData)comp).qrAnchorID;
                                    }
                                    try
                                    {
                                        _qrAnchorContext.SaveChanges();
                                    }
                                    catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                                    {
                                        Console.WriteLine(e);
                                        status = 507;
                                        return;
                                    }                                
                                }
                                break;
                            case "PDF":
                            case "pdf":
                                PDFComponents pdfToUpdate = _pdfContext.PDFComponents.Where(p => p.nodeComponentID == comp.nodeComponentID).SingleOrDefault();
                                if (pdfToUpdate != null)
                                {
                                    if (((PDFNodeComponentData)comp).pdfLink != null)
                                    {
                                        pdfToUpdate.pdfLink = ((PDFNodeComponentData)comp).pdfLink;
                                    }
                                    if (((PDFNodeComponentData)comp).title != null)
                                    {
                                        pdfToUpdate.title = ((PDFNodeComponentData)comp).title;
                                    }
                                    if (((PDFNodeComponentData)comp).description != null)
                                    {
                                        pdfToUpdate.description = ((PDFNodeComponentData)comp).description;
                                    }
                                    try
                                    {
                                        _pdfContext.SaveChanges();
                                    }
                                    catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                                    {
                                        Console.WriteLine(e);
                                        status = 507;
                                        return;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            status = 200;
		}

        //TODO: Implement check that user is only allowed to drop component from 
        //          node in their own business
        /// <summary>
        /// Public method to remove a component from an existing node. 
        /// </summary>
        /// <returns><c>true</c>, if component was dropped successfully, <c>false</c> otherwise.</returns>
        /// <param name="nodeID">Node identifier.</param>
        /// <param name="compID">Node Component identifier.</param>
        public bool DropComponent(int nodeID, int compID, out int status) 
        {
            NodeComponents compToDrop = _compContext.NodeComponents.Where(c => c.id == compID && c.nodeID == nodeID).SingleOrDefault();
            if(compToDrop != null) 
            {
                DropComponentType(compToDrop, out status);
                if (status != 200) {
                    return false;
                }
                return true;
            }
            status = 400;
            return false;
        }

        //TODO: Implement check that user is only allowed to delete
        //          node in their own business
        /// <summary>
        /// Delete a node that exists in the Ario database
        /// </summary>
        /// <returns><c>true</c>, if node was deleted successfully, <c>false</c> otherwise.</returns>
        /// <param name="id">Node Identifier.</param>
        public bool Remove(int id, out int status)
        {
            var nodeToRemove = _nodeContext.Nodes.SingleOrDefault(r => r.id == id);
            if (nodeToRemove != null)
            {
                List<NodeComponents> compList = _compContext.NodeComponents.Where(c => c.nodeID == id).ToList();
                foreach(NodeComponents comp in compList) {
                    DropComponentType(comp, out status);
                    if (status != 200) {
                        return false;
                    }
                }

                List<NodeTeamJoin> nodeTeamList = _nodeTeamContext.NodeTeamJoin.Where(j => j.nodeID == id).ToList();
                foreach(NodeTeamJoin joinEntry in nodeTeamList) {
                    _nodeTeamContext.NodeTeamJoin.Remove(joinEntry);
                }
                try
                {
                    _nodeTeamContext.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e);
                    status = 507;
                    return false;
                }

                _nodeContext.Nodes.Remove(nodeToRemove);
                try
                {
                    _nodeContext.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e);
                    status = 507;
                    return false;
                }
                status = 200;
                return true;
            }
            status = 400;
            return false;
        }

        /// <summary>
        /// Delte the specific component type that corresponds to the 
        /// comp.type passed in the node component data object
        /// </summary>
        /// <param name="comp">Node component data object.</param>
        private void DropComponentType(NodeComponents comp, out int status) {
            switch (comp.type)
            {
                case "Label":
                case "label":
                    Labels label = _labelContext.Labels.Where(l => l.nodeComponentID == comp.id).SingleOrDefault();

                    if (label != null)
                    {
                        _labelContext.Labels.Remove(label);
                        try
                        {
                            _labelContext.SaveChanges();
                        }
                        catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                        {
                            Console.WriteLine(e);
                            status = 507;
                            return;
                        }                    
                    }
                    break;

                case "QRAnchor":
                case "qranchor":
                case "QRanchor":
                    QRAnchorComponents anchor = _qrAnchorContext.QRAnchorComponents.Where(q => q.nodeComponentID == comp.id).SingleOrDefault();

                    if (anchor != null)
                    {
                        _qrAnchorContext.QRAnchorComponents.Remove(anchor);
                        try
                        {
                            _qrAnchorContext.SaveChanges();
                        }
                        catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                        {
                            Console.WriteLine(e);
                            status = 507;
                            return;
                        }                    
                    }
                    break;
                case "PDF":
                case "pdf":
                    PDFComponents pdf = _pdfContext.PDFComponents.Where(p => p.nodeComponentID == comp.id).SingleOrDefault();

                    if (pdf != null)
                    {
                        _pdfContext.PDFComponents.Remove(pdf);
                        try
                        {
                            _pdfContext.SaveChanges();
                        }
                        catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                        {
                            Console.WriteLine(e);
                            status = 507;
                            return;
                        }
                    }
                    break;
            }
            _compContext.NodeComponents.Remove(comp);
            try
            {
                _compContext.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                Console.WriteLine(e);
                status = 507;
                return;
            }
            status = 200;
        }

	}
}

