%% Run DLC 6.1 - Extreme Turbulence Model while Parked with/without Pitch fault

% clear;
A1_Initialize; tic; % Sets up folders and MATLAB path
% load('./SUMR_D_steady_state.mat'); % values used for initial conditions
TemplateDir = [basePath,filesep,'Master']; % Location to find master input files

% Simulation Setup
SimIdlingFeathered = 1;
SimBrakedBladesStuck = 0;
Yaw = [-15:5:15];

% Plotting and Post Processing, DELS, etc.
S_PP=1;         % Switch to run Post Process files 1=run 2=don't run

% Controller/Turbine Setup
% Parameters.Turbine.String = 'SUMR-50_v1';
% Name_Model   = 'BL_CollectivePitch_Shutdown';
% Name_Control = 'C_BL_SUMR50';
Name_FAedit  = 'A3_OFedit_master';  % Fast/Aerodyn Edit Script


% FAST Input File Modifications
FA.TMax         = 300;          % Wind Files are 700s
FA.DT           = 0.0125;
FA.CompAero     = 2;

AD.WakeMod = 0;
AD.SkewMod = 1;


% ED.DrTrDOF = 'False';
% ED.TwFADOF1 = 'False';
% ED.TwFADOF2 = 'False';
% ED.TwSSDOF1 = 'False';
% ED.GenDOF = 'False';

% SD.HSSBrMode = 0;
% SD.YCMode = 0;
% SD.HSSBrTqF = 1E5;

% Disturbance Setup
IW.WindType       = 2;        % 1=steady; 2=uniform; 3=binary TurbSim FF; 4=binary Bladed-style FF; 5=HAWC format; 6=User defined
InflowWindRoot    = 'D:\SUMR_Data\Wind_Files\SUMR-50_Tests\IEC_ClassI\';  % Wind file directory
Disturbance.File  = 'placeholder';
MeanWindSpeeds    = 50;
WindClass         = {'EWM'};
Number            = 1;

% Setup fault parameters (to be used in Simulink to emulate CART2 shutdown)
stuckBlade = 0; % 0 for disabled, 1 for enabled
runawayBlade = 0; % 0 for disabled, 1 for enabled
PitchFaultTime = FA.TMax + 100;
GridLossTime = FA.TMax + 100;
PitchRampRate1 = 18; % Pitch rate used for the 1st 3 seconds of N-stop [deg/s]
PitchRampRate2 = 18; % Pitch rate used for remaining time of N-stop [deg/s]
PitchRunawayRate = 18; % Pitch rate at which blade runs away (pitches uncontrollably) [deg/s]
FaultDelay = 1; % Time it takes for the system to identify a fault [s]
GenTorqueStopRate = 800/1e3; % Rate at which Gen Torque approaches zero during stop (kn-M)

% Save Information
PP.Save.Enable = 1;
PP.Save.Control = 'baseline';
PP.Save.DLC = 'DLC6.1';
if ~exist('AD','var')
    PP.Save.AD_ver = 'AD_v15';
elseif isfield(FA,'CompAero')
    if FA.CompAero == 1
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
            if SimIdlingFeathered == 1
                for iYaw = 1:length(Yaw)
                    
                    Parameters.Turbine.IC.Bp = 90;
                    Parameters.Turbine.IC.Wr = 0;
                    Parameters.Turbine.IC.Wg = 0;
                    Parameters.Turbine.IC.Tg = 0;
                    
                    YawPzn  = Yaw(iYaw)*pi/180;
                    YawRate = 0;
                    
                    ED.BlPitch_1  = Parameters.Turbine.IC.Bp;
                    ED.BlPitch_2  = Parameters.Turbine.IC.Bp;
                    ED.RotSpeed   = Parameters.Turbine.IC.Wr;
                    ED.NacYaw     = Yaw(iYaw);
                    SD.YawNeut    = Yaw(iYaw);
                    SD.TPitManS_1 = 0;
                    SD.TPitManS_2 = 0;
                    SD.BlPitchF_1 = Parameters.Turbine.IC.Bp;
                    SD.BlPitchF_2 = Parameters.Turbine.IC.Bp;
                    
                    % Set Wind File & Save Name
                    PP.Save.WindCase = [WindClass{iWindClass},num2str(MeanWindSpeeds(iMeanWindSpeed))];
                    IW.Filename = [InflowWindRoot,PP.Save.WindCase,'.hh'];
                    PP.Save.Name = [Parameters.Turbine.String,'_',WindClass{iWindClass},'_Idling_Feathered','_Yaw_',num2str(Yaw(iYaw)),'_',num2str(MeanWindSpeeds(iMeanWindSpeed)),'_',num2str(iNumber),'_NoTeeter_SkewMod',num2str(AD.SkewMod)];
                    PP.Save.Dir = ['..',filesep,'..',filesep,'Analysis',filesep,...
                        Parameters.Turbine.String,filesep,PP.Save.Control,filesep,...
                        PP.Save.DLC,filesep,PP.Save.AD_ver,filesep,PP.Save.Teeter];
                    
                    disp(['Simulating Turbine w/ ',IW.Filename,'.bts']);
                    
                    % Run FAST
                    A2_1_Sim_OpenFAST;
                end
            end
            %%
            if SimBrakedBladesStuck == 1
                for iYaw = 1:length(Yaw)
                    
                    Parameters.Turbine.IC.Bp = 0;
                    Parameters.Turbine.IC.Wr = 0;
                    Parameters.Turbine.IC.Wg = 0;
                    Parameters.Turbine.IC.Tg = 0;
                    
                    YawPzn  = Yaw(iYaw)*pi/180;
                    YawRate = 0;
                    
                    ED.BlPitch_1  = Parameters.Turbine.IC.Bp;
                    ED.BlPitch_2  = Parameters.Turbine.IC.Bp;
                    ED.RotSpeed   = Parameters.Turbine.IC.Wr;
                    ED.NacYaw     = Yaw(iYaw);
                    SD.YawNeut    = Yaw(iYaw);
                    SD.THSSBrDp   = 0;
                    SD.TPitManS_1 = 0;
                    SD.TPitManS_2 = 0;
                    SD.BlPitchF_1 = Parameters.Turbine.IC.Bp;
                    SD.BlPitchF_2 = Parameters.Turbine.IC.Bp;

                    % Set Wind File & Save Name
                    PP.Save.WindCase = [WindClass{iWindClass},num2str(MeanWindSpeeds(iMeanWindSpeed))];
                    IW.Filename = [InflowWindRoot,PP.Save.WindCase,'.hh'];
                    PP.Save.Name = [Parameters.Turbine.String,'_',WindClass{iWindClass},'_Braked_0deg','_Yaw_',num2str(Yaw(iYaw)),'_',num2str(MeanWindSpeeds(iMeanWindSpeed)),'_',num2str(iNumber),'_NoTeeter_SkewMod',num2str(AD.SkewMod)];
                    PP.Save.Dir = ['..',filesep,'..',filesep,'Analysis',filesep,...
                        Parameters.Turbine.String,filesep,PP.Save.Control,filesep,...
                        PP.Save.DLC,filesep,PP.Save.AD_ver,filesep,PP.Save.Teeter];
                    
                    disp(['Simulating Turbine w/ ',IW.Filename,'.bts']);
                    
                    % Run FAST
                    A2_1_Sim_OpenFAST;
                    
                    SD = rmfield(SD,'THSSBrDp');
                end
            end
        end
    end
end