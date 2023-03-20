%% Run DLC 1.4 - Extreme Coherent Gust with Direction Change

% clear;
A1_Initialize; tic; % Sets up folders and MATLAB path
% load('./SUMR_D_steady_state.mat'); % values used for initial conditions
TemplateDir = [basePath,filesep,'Master']; % Location to find master input files

% Plotting and Post Processing, DELS, etc.
S_PP=1;         % Switch to run Post Process files 1=run 2=don't run

% Controller/Turbine Setup
% Parameters.Turbine.String = 'SUMR-50_v1';
% Name_Model   = 'BL_CollectivePitch';
% Name_Control = 'C_BL_SUMR50';
Name_FAedit  = 'A3_OFedit_master';  % Fast/Aerodyn Edit Script

% FAST Input File Modifications
FA.TMax         = 300;          % Wind Files are 700s
FA.DT           = 0.0125;
FA.CompElast    = 1;

AD.SkewMod = 1;

% Disturbance Setup
IW.WindType       = 2;        % 1=steady; 2=uniform; 3=binary TurbSim FF; 4=binary Bladed-style FF; 5=HAWC format; 6=User defined
InflowWindRoot    = 'D:\SUMR_Data\Wind_Files\SUMR-50_Tests\IEC_ClassI\';  % Wind file directory
Disturbance.File  = 'placeholder';
ECDCases = {'ECD+R','ECD-R','ECD+R+2.0','ECD+R-2.0','ECD-R+2.0','ECD-R-2.0'};
MeanWindSpeeds = 10;%[5,5,8,4,8,4];

% Setup fault parameters (to be used in Simulink to emulate CART2 shutdown)
stuckBlade = 0; % 0 for disabled, 1 for enabled
runawayBlade = 0; % 0 for disabled, 1 for enabled
PitchFaultTime = FA.TMax + 100;
GridLossTime = FA.TMax + 100;
PitchRampRate1 = 12; % Pitch rate used for the 1st 3 seconds of N-stop [deg/s]
PitchRampRate2 = 2; % Pitch rate used for remaining time of N-stop [deg/s]
PitchRunawayRate = 18; % Pitch rate at which blade runs away (pitches uncontrollably) [deg/s]
FaultDelay = 1; % Time it takes for the system to identify a fault [s]
GenTorqueStopRate = 800/1e3; % Rate at which Gen Torque approaches zero during stop (kn-M)

% Save Information
PP.Save.Enable = 1;
PP.Save.Control = 'baseline';
PP.Save.DLC = 'DLC1.4';
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
PP.Save.Teeter = 'No_Teeter';
PP.Save.Name = 'placeholder';

for iECDCase = 1:length(ECDCases)
    
%     ind = find(SUMR_D_steady_state.wind_speed == MeanWindSpeeds(iECDCase));
%     
%     Parameters.Turbine.IC.Bp = SUMR_D_steady_state.bld_pitch(ind);
%     Parameters.Turbine.IC.Wr = SUMR_D_steady_state.rot_speed(ind);
%     Parameters.Turbine.IC.Wg = SUMR_D_steady_state.gen_speed(ind);
%     Parameters.Turbine.IC.Tg = SUMR_D_steady_state.gen_torque(ind)*1000;
%     
%     ED.BlPitch_1 = Parameters.Turbine.IC.Bp;
%     ED.BlPitch_2 = Parameters.Turbine.IC.Bp;
%     ED.RotSpeed  = Parameters.Turbine.IC.Wr;
    
    % Set Wind File & Save Name
    PP.Save.WindCase = ECDCases{iECDCase};
    IW.Filename = [InflowWindRoot,PP.Save.WindCase,'.hh'];
    PP.Save.Name = [Parameters.Turbine.String,'_',ECDCases{iECDCase},'_NoTeeter_SkewMod1'];
    PP.Save.Dir = ['..',filesep,'..',filesep,'Analysis',filesep,...
        Parameters.Turbine.String,filesep,PP.Save.Control,filesep,...
        PP.Save.DLC,filesep,PP.Save.AD_ver,filesep,PP.Save.Teeter];
    
    disp(['Simulating Turbine w/ ',IW.Filename,'.hh']);
    
    % Run FAST
    A2_1_Sim_OpenFAST;
end