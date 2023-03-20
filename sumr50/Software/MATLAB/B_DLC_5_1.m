%% Run DLC 5.1 - Emergency Shutdown

% clear;
A1_Initialize; tic; % Sets up folders and MATLAB path
% load('./SUMR_D_steady_state.mat'); % values used for initial conditions
TemplateDir = [basePath,filesep,'Master']; % Location to find master input files

% Simulation Setup
SimNStop = 1; % enable to perform normal stop simulations
SimEStop = 1; % enable to perform emergency stop simulations
stopTimes = [200,300,400];

% Plotting and Post Processing, DELS, etc.
S_PP=1;         % Switch to run Post Process files 1=run 2=don't run

% Controller/Turbine Setup
% Parameters.Turbine.String = 'SUMR-50_v1';
Name_Model   = 'BL_CollectivePitch_Shutdown';
% Name_Control = 'C_BL_SUMR50';
Name_FAedit  = 'A3_OFedit_master';  % Fast/Aerodyn Edit Script


% FAST Input File Modifications
FA.TMax         = 700;          % Wind Files are 700s
FA.DT           = 0.0125;

AD.SkewMod = 1;

% Disturbance Setup
IW.WindType       = 3;        % 1=steady; 2=uniform; 3=binary TurbSim FF; 4=binary Bladed-style FF; 5=HAWC format; 6=User defined
InflowWindRoot    = 'D:\SUMR_Data\Wind_Files\SUMR-50_Tests\DLC_1_3_50MW_ClassI\';  % Wind file directory
Disturbance.File  = 'placeholder';
MeanWindSpeeds    = [10,12,24];
WindClass         = {'1ETM_B'};
Number            = 1;

% Setup fault parameters (to be used in Simulink to emulate CART2 shutdown)
stuckBlade = 0; % 0 for disabled, 1 for enabled
runawayBlade = 0; % 0 for disabled, 1 for enabled
PitchFaultTime = FA.TMax + 100;
GridLossTime = FA.TMax + 100;
PitchRampRate1 = 18; % Pitch rate used for the 1st 3 seconds of N-stop [deg/s]
PitchRampRate2 = 18; % Pitch rate used for remaining time of N-stop [deg/s]
PitchRunawayRate = 18; % Pitch rate at which blade runs away (pitches uncontrollably) [deg/s]
FaultDelay = 0; % Time it takes for the system to identify a fault [s]
GenTorqueStopRate = 800/1e3; % Rate at which Gen Torque approaches zero during stop (kn-M)

% Save Information
PP.Save.Enable = 1;
PP.Save.Control = 'baseline';
PP.Save.DLC = 'DLC5.1';
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

for iMeanWindSpeed = 1:length(MeanWindSpeeds)
    for iWindClass = 1:length(WindClass)
        for iNumber = Number
            for istopTimes = 1:length(stopTimes)
                if SimNStop == 1
                    
%                     ind = find(SUMR_D_steady_state.wind_speed == MeanWindSpeeds(iMeanWindSpeed));
%                     
%                     Parameters.Turbine.IC.Bp = SUMR_D_steady_state.bld_pitch(ind);
%                     Parameters.Turbine.IC.Wr = SUMR_D_steady_state.rot_speed(ind);
%                     Parameters.Turbine.IC.Wg = SUMR_D_steady_state.gen_speed(ind);
%                     Parameters.Turbine.IC.Tg = SUMR_D_steady_state.gen_torque(ind)*1000;
%                     
%                     ED.BlPitch_1 = Parameters.Turbine.IC.Bp;
%                     ED.BlPitch_2 = Parameters.Turbine.IC.Bp;
%                     ED.RotSpeed  = Parameters.Turbine.IC.Wr;
                    
                    SD.THSSBrDp    = 9999;
                    SD.TPitManS_1  = stopTimes(istopTimes);
                    SD.TPitManS_2  = stopTimes(istopTimes);
                    SD.PitManRat_1 = 2;
                    SD.PitManRat_2 = 2;
                    SD.BlPitchF_1  = 90;
                    SD.BlPitchF_2  = 90;
                    
                    % Set Wind File & Save Name
                    PP.Save.WindCase = [WindClass{iWindClass},'_',num2str(MeanWindSpeeds(iMeanWindSpeed)),'_',num2str(iNumber)];
                    IW.Filename = [InflowWindRoot,PP.Save.WindCase,'.bts'];
                    PP.Save.Name = [Parameters.Turbine.String,'_',WindClass{iWindClass},'_EStop','_',num2str(MeanWindSpeeds(iMeanWindSpeed)),'_',num2str(iNumber),'_NoTeeter_SkewMod1'];
                    PP.Save.Dir = ['..',filesep,'..',filesep,'Analysis',filesep,...
                        Parameters.Turbine.String,filesep,PP.Save.Control,filesep,...
                        PP.Save.DLC,filesep,PP.Save.AD_ver,filesep,PP.Save.Teeter,filesep];
                    
                    disp(['Simulating Turbine w/ ',IW.Filename,'.bts']);
                    
                    % Run FAST
                    A2_1_Sim_OpenFAST;
                end
                %%
                if SimEStop == 1
                    
%                     ind = find(SUMR_D_steady_state.wind_speed == MeanWindSpeeds(iMeanWindSpeed));
%                     
%                     Parameters.Turbine.IC.Bp = SUMR_D_steady_state.bld_pitch(ind);
%                     Parameters.Turbine.IC.Wr = SUMR_D_steady_state.rot_speed(ind);
%                     Parameters.Turbine.IC.Wg = SUMR_D_steady_state.gen_speed(ind);
%                     Parameters.Turbine.IC.Tg = SUMR_D_steady_state.gen_torque(ind)*1000;
%                     
%                     ED.BlPitch_1 = Parameters.Turbine.IC.Bp;
%                     ED.BlPitch_2 = Parameters.Turbine.IC.Bp;
%                     ED.RotSpeed  = Parameters.Turbine.IC.Wr;
                    
                    SD.THSSBrDp    = stopTimes(istopTimes);
                    SD.TPitManS_1  = stopTimes(istopTimes);
                    SD.TPitManS_2  = stopTimes(istopTimes);
                    SD.PitManRat_1 = 2;
                    SD.PitManRat_2 = 2;
                    SD.BlPitchF_1  = 90;
                    SD.BlPitchF_2  = 90;
                    
                    % Set Wind File & Save Name
                    PP.Save.WindCase = [WindClass{iWindClass},'_',num2str(MeanWindSpeeds(iMeanWindSpeed)),'_',num2str(iNumber)];
                    IW.Filename = [InflowWindRoot,PP.Save.WindCase,'.bts'];
                    PP.Save.Name = [Parameters.Turbine.String,'_',WindClass{iWindClass},'_EStop_18degBrake','_A','_',num2str(MeanWindSpeeds(iMeanWindSpeed)),'_',num2str(iNumber),'_NoTeeter_SkewMod1'];
                    PP.Save.Dir = ['..',filesep,'..',filesep,'Analysis',filesep,...
                        Parameters.Turbine.String,filesep,PP.Save.Control,filesep,...
                        PP.Save.DLC,filesep,PP.Save.AD_ver,filesep,PP.Save.Teeter];
                    
                    disp(['Simulating Turbine w/ ',IW.Filename,'.bts']);
                    
                    % Run FAST
                    A2_1_Sim_OpenFAST;
                end
            end
        end
    end
end