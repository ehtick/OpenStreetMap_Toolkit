/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */
using BH.oM.Geometry;
using BH.oM.Adapters.OpenStreetMap;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using System.Linq;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.OpenStreetMap
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/
        [Description("Convert an OpenStreetMap Way to a Polyline in Universal Transverse Mercator coordinates.")]
        [Input("way", "OpenStreetMap Way to convert.")]
        [Input("gridZone", "Optional Universal Transverse Mercator zone to allow locking conversion to a single zone.")]
        [Output("utmPolyline", "Converted Way as a Polyline.")]
        public static Polyline ToUTMPolyline(this Way way, int gridZone = 0)
        {
            //dictionary to ensure node order is maintained
            ConcurrentDictionary<int, Point> pointDict = new ConcurrentDictionary<int, Point>();
            Parallel.For(0, way.Nodes.Count, n =>
            {
                Point utmPoint = Convert.ToUTMPoint(way.Nodes[n].Latitude, way.Nodes[n].Longitude, gridZone);
                pointDict.TryAdd(n,utmPoint);
            }
            );
            List<Point> points = pointDict.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
            Polyline utmPolyline = Geometry.Create.Polyline(points);
            return utmPolyline;
        }
    }
}



