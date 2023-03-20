%% Run DLC 2.3 - Extreme Operating Gust with Pitch fault

% clear;
A1_Initialize; tic; % Sets up folders and MATLAB path
% load('./SUMR_D_steady_state.mat'); % values used for initial conditions
TemplateDir = [basePath,filesep,'Master']; % Location to find master input files

% Simulation Setup
SimRunawayBlade = 0; % enable to perform runaway blade fault simulations
SimStuckBlade = 1; % enable to perform stuck blade fault simulations
SimGridLoss = 0; % enable to perform grid loss fault simulations

% Plotting and Post Processing, DELS, etc.
S_PP=1;         % Switch to run Post Process files 1=run 2=don't run

% Controller/Turbine Setup
% Parameters.Turbine.String = 'SUMR-50_v1';
Name_Model   = 'BL_CollectivePitch_Shutdown';
% Name_Control = 'C_BL_SUMR50';
Name_FAedit  = 'A3_OFedit_master';  % Fast/Aerodyn Edit Script

% FAST Input File Modifications
FA.TMax         = 300;          % Wind Files are 700s
FA.DT           = 0.0125;

AD.SkewMod = 1;

% Disturbance Setup
IW.WindType       = 2;        % 1=steady; 2=uniform; 3=binary TurbSim FF; 4=binary Bladed-style FF; 5=HAWC format; 6=User defined
InflowWindRoot       = 'D:\SUMR_Data\Wind_Files\SUMR-50_Tests\IEC_ClassI\';  % Wind file directory
Disturbance.File  = 'placeholder';
EOGCases = {'EOGR+2.0','EOGR-2.0','EOGO'};
MeanWindSpeeds = [10];

% Setup fault parameters (to be used in Simulink to emulate CART2 shutdown)
PitchFaultTimeArray = 100;%[200:50:400]; % Time(s) for the pitch fault to occur [s]
GridLossTimeArray = 100;%[200:50:400]; % Time(s) for the grid loss to occur [s]
PitchRampRate1 = 2; % Pitch rate used for the 1st 3 seconds of N-stop [deg/s]
PitchRampRate2 = 2; % Pitch rate used for remaining time of N-stop [deg/s]
PitchRunawayRate = 2; % Pitch rate at which blade runs away (pitches uncontrollably) [deg/s]
FaultDelay = 0; % Time it takes for the system to identify a fault [s]
GenTorqueStopRate = 800/1e3; % Rate at which Gen Torque approaches zero during stop (kn-M)

% Save Information
PP.Save.Enable = 1;
PP.Save.Control = 'baseline';
PP.Save.DLC = 'DLC2.3';
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

for iEOGCase = 1:length(EOGCases)
    if SimRunawayBlade == 1
        for iFaultTime = 1:length(PitchFaultTimeArray)
            stuckBlade = 0; % 0 for disabled, 1 for enabled
            runawayBlade = 1; % 0 for disabled, 1 for enabled
            PitchFaultTime = PitchFaultTimeArray(iFaultTime);
            GridLossTime = FA.TMax + 100;
            
%             ind = find(SUMR_D_steady_state.wind_speed == MeanWindSpeeds(iEOGCase));
%             
%             Parameters.Turbine.IC.Bp = SUMR_D_steady_state.bld_pitch(ind);
%             Parameters.Turbine.IC.Wr = SUMR_D_steady_state.rot_speed(ind);
%             Parameters.Turbine.IC.Wg = SUMR_D_steady_state.gen_speed(ind);
%             Parameters.Turbine.IC.Tg = SUMR_D_steady_state.gen_torque(ind)*1000;
            
            if exist('ED','var')
                ED_old = ED;
            else
                ED_old = struct();
            end
%             ED.BlPitch_1 = Parameters.Turbine.IC.Bp;
%             ED.BlPitch_2 = Parameters.Turbine.IC.Bp;
%             ED.RotSpeed  = Parameters.Turbine.IC.Wr;
            
            % Set Wind File & Save Name
            PP.Save.WindCase = EOGCases{iEOGCase};
            IW.Filename = [InflowWindRoot,PP.Save.WindCase,'.hh'];
            PP.Save.Name = [Parameters.Turbine.String,'_EOG_NStop_12degNoBrake_',EOGCases{iEOGCase},'_NoTeeter_SkewMod1'];
            PP.Save.Dir = ['..',filesep,'..',filesep,'Analysis',filesep,...
                Parameters.Turbine.String,filesep,PP.Save.Control,filesep,...
                PP.Save.DLC,filesep,PP.Save.AD_ver,filesep,PP.Save.Teeter];
            
            disp(['Simulating Turbine w/ ',IW.Filename,'.hh']);
            
            % Run FAST
            A2_1_Sim_OpenFAST;
            
            % Clean up for next simulation
            EDf = fieldnames(ED);
            ED_oldf = fieldnames(ED_old);
            frem = setdiff(EDf,ED_oldf);
            ED = rmfield(ED,frem);
            clear frem ED_oldf ED_old EDf
        end
    end
    
    if SimStuckBlade == 1
        for iFaultTime = 1:length(PitchFaultTimeArray)
            stuckBlade = 1; % 0 for disabled, 1 for enabled
            runawayBlade = 0; % 0 for disabled, 1 for enabled
            PitchFaultTime = PitchFaultTimeArray(iFaultTime);
            GridLossTime = FA.TMax + 100;
            
%             ind = find(SUMR_D_steady_state.wind_speed == MeanWindSpeeds(iEOGCase));
%             
%             Parameters.Turbine.IC.Bp = SUMR_D_steady_state.bld_pitch(ind);
%             Parameters.Turbine.IC.Wr = SUMR_D_steady_state.rot_speed(ind);
%             Parameters.Turbine.IC.Wg = SUMR_D_steady_state.gen_speed(ind);
%             Parameters.Turbine.IC.Tg = SUMR_D_steady_state.gen_torque(ind)*1000;
%             
            if exist('ED','var')
                ED_old = ED;
            else
                ED_old = struct();
            end
%             ED.BlPitch_1 = Parameters.Turbine.IC.Bp;
%             ED.BlPitch_2 = Parameters.Turbine.IC.Bp;
%             ED.RotSpeed  = Parameters.Turbine.IC.Wr;
%             
            % Set Wind File & Save Name
            PP.Save.WindCase = EOGCases{iEOGCase};
            IW.Filename = [InflowWindRoot,PP.Save.WindCase,'.hh'];
            PP.Save.Name = [Parameters.Turbine.String,'_EOG_NStop_12degNoBrake_',EOGCases{iEOGCase},'_NoTeeter_SkewMod1'];
            PP.Save.Dir = ['..',filesep,'..',filesep,'Analysis',filesep,...
                Parameters.Turbine.String,filesep,PP.Save.Control,filesep,...
                PP.Save.DLC,filesep,PP.Save.AD_ver,filesep,PP.Save.Teeter];
            
            disp(['Simulating Turbine w/ ',IW.Filename,'.hh']);
            
            % Run FAST
            A2_1_Sim_OpenFAST;
            
            % Clean up for next simulation
            EDf = fieldnames(ED);
            ED_oldf = fieldnames(ED_old);
            frem = setdiff(EDf,ED_oldf);
            ED = rmfield(ED,frem);
            clear frem ED_oldf ED_old EDf
        end
    end
    %%
    if SimGridLoss == 1
        for iGridLossTime = 1:length(GridLossTimeArray)
            stuckBlade = 0; % 0 for disabled, 1 for enabled
            runawayBlade = 0; % 0 for disabled, 1 for enabled
            GridLossTime = GridLossTimeArray(iGridLossTime);
            PitchFaultTime = FA.TMax + 100;
            
            if exist('SD','var')
                SD_old = SD;
            else
                SD_old = struct();
            end
            SD.TPitManS_1  = GridLossTime + FaultDelay; % time at which to begin pitching
            SD.TPitManS_2  = GridLossTime + FaultDelay; % time at which to begin pitching
            SD.PitManRat_1 = 2; % pitch rate used by ServoDyn during grid loss [deg/s]
            SD.PitManRat_2 = 2; % pitch rate used by ServoDyn during grid loss [deg/s]
            SD.BlPitchF_1  = 90; % final pitch position for pitch maneuver [deg]
            SD.BlPitchF_2  = 90; % final pitch position for pitch maneuver [deg]
            SD.THSSBrDp    = GridLossTime + FaultDelay;
            SD.HSSBrTqF    = 9163; % Brake torque used by ServoDyn during E-stop [N-m]
            SD.HSSBrDT     = 0.15; % Time it takes to fully apply the brake [s]
%             
%             ind = find(SUMR_D_steady_state.wind_speed == MeanWindSpeeds(iEOGCase));
%             
%             Parameters.Turbine.IC.Bp = SUMR_D_steady_state.bld_pitch(ind);
%             Parameters.Turbine.IC.Wr = SUMR_D_steady_state.rot_speed(ind);
%             Parameters.Turbine.IC.Wg = SUMR_D_steady_state.gen_speed(ind);
%             Parameters.Turbine.IC.Tg = SUMR_D_steady_state.gen_torque(ind)*1000;
%             
            if exist('ED','var')
                ED_old = ED;
            else
                ED_old = struct();
            end
%             ED.BlPitch_1 = Parameters.Turbine.IC.Bp;
%             ED.BlPitch_2 = Parameters.Turbine.IC.Bp;
%             ED.RotSpeed  = Parameters.Turbine.IC.Wr;
            
            % Set Wind File & Save Name
            PP.Save.WindCase = EOGCases{iEOGCase};
            IW.Filename = [InflowWindRoot,PP.Save.WindCase,'.hh'];
            PP.Save.Name = [Parameters.Turbine.String,'_EOG_EStop_Brake_0deg_',EOGCases{iEOGCase},'_NoTeeter_SkewMod1'];
            PP.Save.Dir = ['..',filesep,'..',filesep,'Analysis',filesep,...
                Parameters.Turbine.String,filesep,PP.Save.Control,filesep,...
                PP.Save.DLC,filesep,PP.Save.AD_ver,filesep,PP.Save.Teeter];
            
            disp(['Simulating Turbine w/ ',IW.Filename,'.hh']);
            
            % Run FAST
            A2_1_Sim_OpenFAST;
            
            % Clean up for next simulation
            EDf = fieldnames(ED);
            ED_oldf = fieldnames(ED_old);
            frem = setdiff(EDf,ED_oldf);
            ED = rmfield(ED,frem);
            clear frem ED_oldf ED_old EDf
            SDf = fieldnames(SD);
            SD_oldf = fieldnames(SD_old);
            frem = setdiff(SDf,SD_oldf);
            SD = rmfield(SD,frem);
            clear frem SD_oldf SD_old SDf
        end
    end
end