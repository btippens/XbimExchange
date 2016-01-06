﻿using System.Collections.Generic;
using System.Linq;
using Xbim.CobieExpress;
using Xbim.Common;
using Xbim.FilterHelper;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using XbimExchanger.IfcToCOBieExpress.Classifications;

namespace XbimExchanger.IfcToCOBieExpress
{
    public class IfcToCoBieExpressExchanger : XbimExchanger<IfcStore, IModel>
    {
        private readonly bool _classify;
        internal COBieExpressHelper Helper ;
        /// <summary>
        /// Instantiates a new IIfcToCOBieLiteUkExchanger class.
        /// </summary>
        public IfcToCoBieExpressExchanger(IfcStore source, IModel target, ReportProgressDelegate reportProgress = null, OutPutFilters filter = null, string configFile = null, EntityIdentifierMode extId = EntityIdentifierMode.IfcEntityLabels, SystemExtractionMode sysMode = SystemExtractionMode.System | SystemExtractionMode.Types, bool classify = false) 
            : base(source, target)
        {
            ReportProgress.Progress = reportProgress; //set reporter
            Helper = new COBieExpressHelper(source, ReportProgress, filter, configFile, extId, sysMode);
            this._classify = classify;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IModel Convert()
        {
            var mapping = GetOrCreateMappings<MappingIfcBuildingToFacility>();
            var buildings = SourceRepository.Instances.OfType<IIfcBuilding>().ToList();
            var facilities = new List<CobieFacility>(buildings.Count);
            foreach (var ifcBuilding in buildings)
            {
                var facility = TargetRepository.Instances.New<CobieFacility>();
                facility = mapping.AddMapping(ifcBuilding, facility);
                if(_classify)       
                    facility.Classify();
                facilities.Add(facility);
            }
            return TargetRepository;
        }
    }
}