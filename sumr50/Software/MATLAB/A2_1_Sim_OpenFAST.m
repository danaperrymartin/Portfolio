%% A2_1_Sim
% created by J.Aho 9/22/11
% modified by D.Zalkind 3/2015
% modified by D.Martin 5/2016
% modified by Christopher Bay 12/5/2017

% A script that will set up a simulation with specified:
% Model, Controller, FAEdit script, Post Processing file, Plot file and
% run TurbSim or user defined wind files.

if 1
%% Post Processing Parameters (Plotting/Post Processing)
clear all;
close all;

A1_Initialize; tic; % Sets up folders and MATLAB path
% IC = load('./SUMR_D_v3.mat'); % values used for initial conditions
TemplateDir = [basePath,filesep,'Master']; % Location to find master input files

Parameters.Turbine.String = 'SUMR-D';
Name_Model                = 'BL_CollectivePitch';%'BL_CollectivePitch';        
Name_Control              = 'C_BL_Demonstrator';
Name_FAedit               = 'A3_OFedit_master';  % Fast/Aerodyn Edit Script

FA.TMax         = 200;          % Wind Files are 700s
FA.DT           = 0.0125;
FA.CompElast    = 1;            % 1 ElastoDyn, 2 ElastoDyn+BeamDyn for Blades
AD.SkewMod      = 1;

%% Disturbance Setup
    IW.WindType             = 2;        % 1=steady; 2=uniform; 3=binary TurbSim FF; 4=binary Bladed-style FF; 5=HAWC format; 6=User defined
    Disturbance.Type        = 5;
    IW.HWindSpeed           = 5;
    InflowWindRoot          = [basePath,'\Analysis\WindFiles\SUMR-D_Tests\const\'];  % Wind file directory                            % Wind file directory
    IW.Filename             = 'sstep.wnd';              
    Disturbance.Vshear      = 0; 
    Disturbance.LVshear     = 0;
    Disturbance.LHshear     = 0;
    Disturbance.U_ref       = 4;
    Disturbance.Step        = 1;
    Disturbance.Tgust       = 15;
    Disturbance.Vgust       = 2;
    Disturbance.Tstart      = 100;
    Disturbance.Tend        = 100;       % Wind files are ~700s
    Disturbance.freq        = 0.4640;   %[0,0.4640,1.4640,2.4640]
    Disturbance.amp         = 2;
 %% Save Information
    PP.Save.Dir = ['../../Analysis/',Parameters.Turbine.String,'/step_4to5/'];
    PP.Save.Name = [Parameters.Turbine.String,'_','const_',num2str(IW.HWindSpeed)];
    PP.Save.Enable = 1;   
end
%% Post Processing Parameters (Plotting/Post Processing)
% Plotting and Post Processing, DELS, etc.
S_PP=1;         % Switch to run Post Process files 1=run 2=don't run

if S_PP==1 %Parameters only used if Post Processing is turned on
    pp=1;
    
    %% Calculated Channels
    %     Name_PP{pp}             = 'A4_1_Plot_Channels'; pp=pp+1;
    %     PP.CalcCh(1).name       = 'TwrBs';
    %     PP.CalcCh(1).eval       = 'sqrt(OutData(:,$TwrBsMyt$).^2+OutData(:,$TwrBsMxt).^2)';
    %
    %% Plot Channels
    Name_PP{pp}             =   'A4_8_Plot_Channels'; pp=pp+1;
    PP.Channels{1}          =   {
        {'Wind1VelX',             []}
        {'Wind1VelY',             []}
        {'Wind1VelZ',             []}
        {'BldPitch1',           []}
        {'GenTq',               []}
        {'RotSpeed',            []}
        {'GenPwr',              []}
        };
    
    PP.Channels{2}          =   {
        {'GenSpeed',           []}
        {'GenTq',           []}
        };
    
    PP.Channels{3}          =   {
        {'YawPzn',           []}
        {'YawVzn',           []}
        {'YawAzn',           []}
        };
    
    PP.Channels{4}          =   {
        {'Wind1VelX',        []}
        };
    
    PP.Channels{5}          =   {
        {'TwrBsMyt',        []}
        {'TwrBsMxt',        []}
        };
    
    PP.Channels{6}          =   {
        {'Wind1VelX'        []}
        {'BldPitch1',       []}
        {'GenSpeed',        []}
        };
    
    PP.Channels{7}          =  {
        {'BldPitch1',          []}
        {'BldPitch2',           []}
        };
    
    PP.Channels{8}          = {
        {'LSShftMxa',       []}};
    
    PP.Channels{9}          =   {
        {'LSShftMxa',       []}
        {'LSSTipMzs',        []}
        {'LSSGagMzs',        []}
        };
    
    PP.Channels{10}          =   {
        {'TTDspFA',          []}
        {'TTDspSS',          []}
        };
    
    PP.Channels{11}          =   {
        {'LSSTipMya',       []}
        {'LSSTipMza',       []}
        {'LSSTipMys',       []}
        };
    
    PP.Channels{12}          =   {
        {'RootMxb1',       []}
        {'RootMyb1',        []}
        {'RootMxb2',       []}
        {'RootMyb2',        []}
        };
    
    PP.Channels{13}          =   {
        {'RootFxb1',       []}
        {'RootFyb1',        []}
        {'RootFxb2',       []}
        {'RootFyb2',        []}
        };
    
    PP.Channels{14}          =   {
        {'OoPDefl1',       []}
        {'OoPDefl2',        []}
        };
    
    PP.Channels{15}          =   {
        {'RtSkew',       []}
        {'RotSpeed',     []}
        {'Azimuth',      []}
        {'HSSBrTq',      []}
        };
    
    PP.TimePlot.Xlim        =   [5 , FA.TMax];
    PP.TimePlot.hold        =   1;
    %     PP.TimePlot.SaveFlag    =   0;
    %     PP.TImePlot.SaveName    =   [];
    
    %% Plot Signals
    %Name_PP{pp} = 'A4_8_Plot_Signals'; pp=pp+1;
    
    PP.Signals{1}             = {
        {'genSpeed_sp',    [],         [],     'genSpeed_{sp}'}
        {'genSpeed_err',    [],         [],     'genSpeed_{err}'}
        {'torque_raw',    [],         [],     'torque_{raw}'}
        
        };
    
    PP.Signals{2}             = {
        {'region',    [],         [],     []}
        {'Tg',      [],         [],   []}
        {'rot_pwr',      [],         [],     []}
        };
    
    %% Power Spectral Density
     %Name_PP{pp} = 'A4_1_PSD'; pp=pp+1;
    
    PP.PSD(1).Signal    = 'GenPwr';
    PP.PSD(1).Duration  = 600;
    %
    PP.PSD(2).Signal    = 'LSShftMxa';
    PP.PSD(2).Duration  = 600;
    
    PP.PSD(3).Signal    = 'RotSpeed';
    PP.PSD(3).Duration  = 600;
    %
    PP.PSD(4).Signal    = 'GenTq';
    PP.PSD(4).Duration  = 600;    
    
    %% Statistics
    %Name_PP{pp} = 'A4_8_Statistics'; pp=pp+1;
    
    sIndex = 1;
    
    PP.Stat(sIndex).Signal = 'GenPwr';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'GenSpeed';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'RootMyc1';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'TwrBsMyt';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'LSShftMxa';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'GenTq';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'BldPitch1';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'RotPwr';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'RotTorq';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'RotThrust';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    %     PP.Stat(sIndex).Signal = 'TSR';
    %     PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'OoPDefl1';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'IPDefl1';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'TTDspFA';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'TTDspSS';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    PP.Stat(sIndex).Signal = 'Wind1VelX';
    PP.Stat(sIndex).Duration = 100; sIndex=sIndex+1;
    
    
    %
    %     PP.Stat(1).Signal    = 'RootMyb1';
    %     PP.Stat(1).Duration  = 120;
    %
    %     PP.Stat(2).Signal    = 'RootMxb1';
    %     PP.Stat(2).Duration  = 300;
    %
    %     PP.Stat(3).Signal    = 'TwrBsMyt';
    %     PP.Stat(3).Duration  = 300;
    %
    %     PP.Stat(4).Signal    = 'TwrBsMxt';
    %     PP.Stat(4).Duration  = 300;
    %
    %     PP.Stat(5).Signal    = 'LSShftMxa';
    %     PP.Stat(5).Duration  = 300;
    %
    %     PP.Stat(6).Signal    = 'YawBrMzp';
    %     PP.Stat(6).Duration  = 300;
    %
    %     PP.Stat(7).Signal   = 'GenPwr';
    %     PP.Stat(7).Duration = 300;
    %
    %     PP.Stat(8).Signal   = 'YawBrMyn';
    %     PP.Stat(8).Duration = 300;
    %
    %     Name_PP{1} = 'A4_Basic_Plots';
    %     Name_PP{3} = 'A4_Controller_Plots';
    
    
    %% DEL
    %     Name_PP{pp} = 'A4_1_DEL'; pp=pp+1;
    %
    %     PP.DEL(1).Signal    = 'RootMyb1';
    %     PP.DEL(1).Duration  = 600;
    %     PP.DEL(1).m         = 10;
    %
    %     PP.DEL(2).Signal    = 'RootMxb1';
    %     PP.DEL(2).Duration  = 300;
    %     PP.DEL(2).m         = 10;
    %
    %     PP.DEL(3).Signal    = 'TwrBsMyt';
    %     PP.DEL(3).Duration  = 300;
    %     PP.DEL(3).m         = 4;
    %
    %     PP.DEL(4).Signal    = 'TwrBsMxt';
    %     PP.DEL(4).Duration  = 300;
    %     PP.DEL(4).m         = 4;
    %
    %     PP.DEL(5).Signal    = 'LSShftMxa';
    %     PP.DEL(5).Duration  = 300;
    %     PP.DEL(5).m         = 5;
    %
    %  No yaw and tilt bearing moment yet, I'll read up
    
    %% Controller Data Script
%     Name_PP{pp} = 'A4_8_BL_Control_Data'; pp=pp+1;
    
    %% AeroDyn Data Script
    %     Name_PP{pp} = 'A4_1_Get_Aero'; pp=pp+1;
    
    %% Energy Quality Script
    %       Name_PP{pp} = 'A4_1_Energy_Quality'; pp=pp+1;
    %       PP.Energy.Time = [100,630];
    
    %% Metrics
    %Name_PP{pp} = 'A4_8_OverspeedMetrics'; pp=pp+1;
    %Name_PP{pp} = 'A4_8_EnergyMetrics'; pp=pp+1;
    
    %% Save Cp Curves
    %       Name_PP{pp}   = 'A4_8_CtCp'; pp=pp+1;
    
    %% Save Results Script (must go last in PP to save *all* results)
    % Save Results Script
    if PP.Save.Enable
    Name_PP{pp} = 'A4_8_SaveData'; pp=pp+1;
    end
end

%% Wind File Input Parameters

% Select Type of Wind File to be used (NOTE: IEC Wind Currently not
% supported, but can replace the TSWind variables.)

% S_WindMode=5;% -2=IEC, -1=Full-Field TurbSim, 0= Use Turbsim input, 1=const 2=step, 3=ramp, 4=sinusoidal
DT = FA.DT;


if IW.WindType==-1
    %     DT  = 0.0125;   % Simulation time step
    Wtype = 'FF';
    
    % Turbulent HH Wind Files
elseif IW.WindType==0 % Set the time parameters for Turbsim Files
    % Nothing is done here
    %     DT      = 0.0125;
    Wtype   = 'HH';
    
elseif IW.WindType~=0 && IW.WindType~=1 && IW.WindType~=3% Set the time parameters for user defined or input files
    WindParams1 = zeros(1,8);  WindParams2=zeros(1,6);
    Wind_Files  = 1; %A vector of wind files to simulate (should be a scalar 1 for non-turbsim files)
    TMax        = FA.TMax;       % Max time of simulation
    DT          = FA.DT;         % Time Step for Simulation
    Vshear      = Disturbance.Vshear;%0.14; % Vertical Power Law Shear, Set to -1 to use \Data\Vshear.m (or change path below) as time varying shear, must be long enough
    Hshear      = Disturbance.LHshear;       % Horizontal Linear Shear, Set to -1 to use \Data\Hshear.m (or change path below) as time varying shear, must be long enough
    GS          = 0;           % Gust Speed
    VS          = 0;           % Vertical Speed
    WD          = 0;           % Wind Direction
    LVshear     = Disturbance.LVshear;      % Linear Vertical Shear, Set to -1 to use \Data\LVshear.m (or change path below) as time varying shear, must be long enough
    WindParams1 = [TMax, DT, WD, VS, Hshear, Vshear, LVshear, GS];
    if Disturbance.Type==1 % Constant Wind
        WindParams2(1)=[Disturbance.U_ref]; % [DCvalue]
        
    elseif Disturbance.Type==2 % Stepped input from Vmin to Vmax then back down in steps of dWind
        WindParams2(1:3)=[5,10,.5]; % [Vmin, Vmax, dWind]
        
    elseif Disturbance.Type==3 % Ramp wind going from Vmin to Vmax to min, ramp starts at Stime
        WindParams2(1:4)=[5,10,50,150];   % [Vmin, Vmax, Stime]
        
    elseif Disturbance.Type==4 % Sinusoidal wind of DC+amp*sin(2*pi*freq*t)
        WindParams2(1:3)=[Disturbance.U_ref,Disturbance.freq,Disturbance.amp];    % [DC, freq (Hz), amp]
        
    elseif Disturbance.Type==5 % Single Step Input
        %                   [ Init. Speed           Ramp Start              Final Speed                             Ramp End   ]
        WindParams2(1:4)  = [ Disturbance.U_ref     Disturbance.Tstart      Disturbance.U_ref + Disturbance.Step    Disturbance.Tend];
        
    elseif Disturbance.Type==6 % Yawed Inflow
        %                   [ Wind Speed            Initial Yaw         Start Time                 Final Yaw           End Time ]
        WindParams2(1:5)  = [ Disturbance.U_ref     0                  Disturbance.Tstart              180                 Disturbance.Tend  ];
        
    elseif Disturbance.Type==7  %Extreme Gust
        %                   [ U_ref                 Start Time             Period              Gust Speed
        WindParams2(1:4)  = [ Disturbance.U_ref     Disturbance.Tstart     Disturbance.Tgust   Disturbance.Vgust];
    
    elseif Disturbance.Type==8 %Extreme Gust with Direction Change
        %                     [ Wind Speed (Vmin)            Start Time            Vmax                 Rise Time]
        WindParams2(1:4) = [Disturbance.U_ref             Disturbance.Tstart    Disturbance.U_ref+15        10];
    
    end
end    
    % Set parameters for choice of user defined wind.  See 'Af_MakeWind.m'
    % for more detailed comments.
    
    
   
%     [Wtype,W]=Af_MakeWind(WindDir,IW.WindType, WindParams1(1),WindParams1(2),WindParams1(3),WindParams1(4),Hshear,Vshear,LVshear,WindParams1(8),WindParams2,0);
%     clear WindParams1 WindParams2  Vshear Hshear GS VS WD LVshear

%% Save Relevant Data to Structs
Simulation.Name_Control = Name_Control;
Simulation.Name_FAedit  = Name_FAedit;
Simulation.Name_Model   = Name_Model;

PP.Scripts              = Name_PP;
PP.Enabled              = S_PP;

%% Run A0_MAIN
    S_RunMode=1;
    A0_MAIN
