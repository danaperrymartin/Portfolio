%% Run Constant Wind Simulation
% clear;
% close all;
A1_Initialize; tic; % Sets up folders and MATLAB path
% load('./SUMR_D_steady_state.mat'); % values used for initial conditions
TemplateDir = [basePath,filesep,'Master']; % Location to find master input files

%% Plotting and Post Processing, DELS, etc. %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
S_PP=1;         % Switch to run Post Process files 1=run 0=don't run

%% Controller/Turbine Setup %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Parameters.Turbine.String = 'SUMR-50_v2';   % Name of turbine parameter set from GoogleDrive sheet
% Parameters.Turbine.ConeAngle = 12.5;    % Coning angle for simulation to use
% Name_Model   = 'BL_CollectivePitch';  % Name of Simulink model to use
% Name_Control = 'C_BL_SUMR50';     % Control script to use (located in ControlFiles)
Name_FAedit  = 'A3_OFedit_master';      % Fast/Aerodyn Edit Script (shouldn't need to modify)

% %% FAST Master Template Files to use %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Name_FAST_Def   = fullfile(TemplateDir,'SUMR-D_FA');        %  FAST input file
% Name_ED_Def     = fullfile(TemplateDir,'SUMR-D_ED_v7');     % ElastoDyn input file
% Name_AD15_Def   = fullfile(TemplateDir,'SUMR-D_AD15_v5');   % AeroDyn15 input file
% Name_AD14_Def   = fullfile(TemplateDir,'SUMR-D_AD14_v4');   % AeroDyn14 input file
% Name_IW_Def     = fullfile(TemplateDir,'SUMR-D_IW');        % InflowWind input file
% Name_SD_Def     = fullfile(TemplateDir,'SUMR-D_SD');        % ServoDyn input file
% Name_BD_Def     = fullfile(TemplateDir,'SUMR-D_BD_s4_v3_Provisional_Primary_test'); % BeamDyn input file


%% FAST Input File Modifications %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
FA.TMax      = 150;     % We tend to use Wind Files that are 700s
FA.DT        = 0.0125;  % The timestep at which OpenFAST will simulate

AD.SkewMod   = 1;       % Used with ADv15; set to 1 if skew angle of inflow > 45 deg

%% Disturbance Setup %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    IW.WindType       = 1;        % 1=steady; 2=uniform; 3=binary TurbSim FF; 4=binary Bladed-style FF; 5=HAWC format; 6=User defined
    WindCase          = [5:1:25];
    InflowWindRoot    = [basePath,'\Analysis\WindFiles\const\'];  
    MeanWindSpeeds    = 5; % Closest average wind speed to the values in the '*_steady_state.mat' file
%     WindCaseFile     = dir([InflowWindRoot,WindCases{1},'*']); % Used for determine wind file extension
%     [~,~,WindCaseExt] = fileparts([WindCaseFile.folder,WindCaseFile.name]); % Used for determine wind file extension
    Disturbance.File  = 'placeholder'; % A placeholder; do not need to change normally


%% Save Information %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
PP.Save.Enable = 1;
PP.Save.Control = Name_Model;
PP.Save.DLC = 'Const';
PP.Save.Teeter = 'No_Teeter';
if ~exist('AD','var')
    PP.Save.AD_ver = 'AD_v15';
elseif isfield(AD,'CompAero')
    if AD.CompAero == 1
        PP.Save.AD_ver = 'AD_v14';
    else
        PP.Save.AD_ver = 'AD_v15';
    end
else
    PP.Save.AD_ver = 'AD_v15';
end
PP.Save.Name = 'placeholder';


%% Run Simulations %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
for iWindCase = 1:length(WindCase)
      
    %%%%% Set Initial Conditions for Simulation %%%%%
%     ind = find(SUMR_D_steady_state.wind_speed == MeanWindSpeeds(iWindCase));
    
%     Parameters.Turbine.IC.Bp = SUMR_D_steady_state.bld_pitch(ind);
%     Parameters.Turbine.IC.Wr = SUMR_D_steady_state.rot_speed(ind);
%     Parameters.Turbine.IC.Wg = SUMR_D_steady_state.gen_speed(ind);
%     Parameters.Turbine.IC.Tg = SUMR_D_steady_state.gen_torque(ind)*1000;
    
%     ED.BlPitch_1 = Parameters.Turbine.IC.Bp;
%     ED.BlPitch_2 = Parameters.Turbine.IC.Bp;
%     ED.RotSpeed  = Parameters.Turbine.IC.Wr;
    
    %%%%% Set Wind File & Save Name %%%%%
    PP.Save.WindCase = WindCase(iWindCase);
%     IW.Filename    = [InflowWindRoot,PP.Save.WindCase,WindCaseExt];
    IW.HWindSpeed    = WindCase(iWindCase);
    PP.Save.Name     = [Parameters.Turbine.String,'_',num2str(WindCase(iWindCase))];
    PP.Save.Dir      = ['..',filesep,'..',filesep,'Analysis',filesep,...
                        Parameters.Turbine.String,filesep,PP.Save.Control,filesep,...
                        PP.Save.DLC,filesep,PP.Save.AD_ver,filesep,PP.Save.Teeter,...
                        filesep,'Const',filesep];
          
    %%%%% Run FAST %%%%%
    disp(['Simulating Turbine w/ ',num2str(WindCase(iWindCase)),'m/s inflow']);
    A2_1_Sim_OpenFAST;
end