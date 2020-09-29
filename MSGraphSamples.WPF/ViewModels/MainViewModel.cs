﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Graph;
using MsGraph_Samples.Helpers;
using MsGraph_Samples.Services;

namespace MsGraph_Samples.ViewModels
{
    public class MainViewModel : Observable
    {
        private readonly IGraphDataService _graphDataService;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public IReadOnlyList<string> Entities => new[] { "Users", "Groups", "Applications", "Devices" };
        private string _selectedEntity = "Users";
        public string SelectedEntity
        {
            get => _selectedEntity;
            set => Set(ref _selectedEntity, value);
        }

        private DirectoryObject? _selectedObject = null;
        public DirectoryObject? SelectedObject
        {
            get => _selectedObject;
            set => Set(ref _selectedObject, value);
        }
        public string? LastUrl => _graphDataService.LastUrl;

        private IEnumerable<DirectoryObject>? _directoryObjects;
        public IEnumerable<DirectoryObject>? DirectoryObjects
        {
            get => _directoryObjects;
            set
            {
                Set(ref _directoryObjects, value);
                SelectedEntity = DirectoryObjects switch
                {
                    GraphServiceUsersCollectionPage _ => "Users",
                    GraphServiceGroupsCollectionPage _ => "Groups",
                    GraphServiceApplicationsCollectionPage _ => "Applications",
                    GraphServiceDevicesCollectionPage _ => "Devices",
                    _ => SelectedEntity,
                };
            }
        }

        public string Select { get; set; } = "id, displayName, mail, userPrincipalName";

        public string Filter { get; set; } = string.Empty;

        public string Search { get; set; } = string.Empty;

        private string _orderBy = "displayName";
        public string OrderBy
        {
            get => _orderBy;
            set => Set(ref _orderBy, value);
        }

        public MainViewModel(IGraphDataService dataService)
        {
            _graphDataService = dataService;
            LoadAction();
        }

        public RelayCommand LoadCommand => new RelayCommand(LoadAction);
        private async void LoadAction()
        {
            IsBusy = true;

            try
            {
                DirectoryObjects = SelectedEntity switch
                {
                    "Users" => await _graphDataService.GetUsersAsync(Filter, Search, Select, OrderBy),
                    "Groups" => await _graphDataService.GetGroupsAsync(Filter, Search, Select, OrderBy),
                    "Applications" => await _graphDataService.GetApplicationsAsync(Filter, Search, Select, OrderBy),
                    "Devices" => await _graphDataService.GetDevicesAsync(Filter, Search, Select, OrderBy),
                    _ => throw new NotImplementedException("Can't find selected entity"),
                };

            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, ex.Error.Message);
            }

            RaisePropertyChanged(nameof(LastUrl));
            IsBusy = false;
        }

        public RelayCommand<DataGridAutoGeneratingColumnEventArgs> AutoGeneratingColumn =>
            new RelayCommand<DataGridAutoGeneratingColumnEventArgs>((e) => e.Cancel = !e.PropertyName.In(Select.Split(',')));

        private RelayCommand<DataGridSortingEventArgs>? _sortCommand;
        public RelayCommand<DataGridSortingEventArgs> SortCommand => _sortCommand ??= new RelayCommand<DataGridSortingEventArgs>(SortAction);
        private void SortAction(DataGridSortingEventArgs e)
        {
            OrderBy = $"{e.Column.Header}";
            e.Handled = true;
            LoadAction();
        }

        private RelayCommand? _drillDownCommand;
        public RelayCommand DrillDownCommand => _drillDownCommand ??= new RelayCommand(DrillDownCommandAction);
        private async void DrillDownCommandAction()
        {
            if (SelectedObject == null)
                return;

            IsBusy = true;

            Filter = string.Empty;

            try
            {
                DirectoryObjects = SelectedEntity switch
                {
                    "Users" => await _graphDataService.GetTransitiveMemberOfAsGroupsAsync(SelectedObject.Id),
                    "Groups" => await _graphDataService.GetTransitiveMembersAsUsersAsync(SelectedObject.Id),
                    "Applications" => await _graphDataService.GetAppOwnersAsUsersAsync(SelectedObject.Id),
                    "Devices" => await _graphDataService.GetTransitiveMemberOfAsGroupsAsync(SelectedObject.Id),
                    _ => null
                };

            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message, ex.Error.Message);
            }

            RaisePropertyChanged(nameof(LastUrl));
            IsBusy = false;
        }

        private RelayCommand? _graphExplorerCommand;
        public RelayCommand GraphExplorerCommand => _graphExplorerCommand ??= new RelayCommand(GraphExplorerAction);
        private void GraphExplorerAction()
        {
            if (LastUrl == null)
                return;

            var geBaseUrl = "https://developer.microsoft.com/en-us/graph/graph-explorer"; 
            var version = "v1.0";
            var graphUrl = "https://graph.microsoft.com";
            var encodedUrl = WebUtility.UrlEncode(LastUrl.Substring(LastUrl.NthIndexOf('/', 4)));
            var encodedHeaders = "W3sibmFtZSI6IkNvbnNpc3RlbmN5TGV2ZWwiLCJ2YWx1ZSI6ImV2ZW50dWFsIn1d";            
            var url = $"{geBaseUrl}?request={encodedUrl}&method=GET&version={version}&GraphUrl={graphUrl}&headers={encodedHeaders}";

            var psi = new System.Diagnostics.ProcessStartInfo { FileName = url, UseShellExecute = true };
            System.Diagnostics.Process.Start(psi);
        }
    }
}