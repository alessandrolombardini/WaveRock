﻿#pragma checksum "..\..\Aggiungi.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3538BF7585071EB38348A7C1925DEB2D975E839EAE2B53DB7973BF3A67C3ECE1"
//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WaveRock;


namespace WaveRock {
    
    
    /// <summary>
    /// Aggiungi
    /// </summary>
    public partial class Aggiungi : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 44 "..\..\Aggiungi.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btt_tornaIndietro;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\Aggiungi.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Frame frame_content;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\Aggiungi.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btt_aggiungiPersona;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\Aggiungi.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btt_aggiungiEdizioneCorso;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\Aggiungi.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btt_aggiungiLezione;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WaveRock;component/aggiungi.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Aggiungi.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.btt_tornaIndietro = ((System.Windows.Controls.Button)(target));
            
            #line 44 "..\..\Aggiungi.xaml"
            this.btt_tornaIndietro.Click += new System.Windows.RoutedEventHandler(this.Btt_tornaIndietro_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.frame_content = ((System.Windows.Controls.Frame)(target));
            return;
            case 3:
            this.btt_aggiungiPersona = ((System.Windows.Controls.Button)(target));
            
            #line 48 "..\..\Aggiungi.xaml"
            this.btt_aggiungiPersona.Click += new System.Windows.RoutedEventHandler(this.Btt_aggiungiPersona_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btt_aggiungiEdizioneCorso = ((System.Windows.Controls.Button)(target));
            
            #line 49 "..\..\Aggiungi.xaml"
            this.btt_aggiungiEdizioneCorso.Click += new System.Windows.RoutedEventHandler(this.Btt_aggiungiEdizioneCorso_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btt_aggiungiLezione = ((System.Windows.Controls.Button)(target));
            
            #line 50 "..\..\Aggiungi.xaml"
            this.btt_aggiungiLezione.Click += new System.Windows.RoutedEventHandler(this.Btt_aggiungiLezione_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

