%% A2_4_Lin_FAST8
% Script created by Dana Martin to run linearizations
    clear;
    tic
    A1_Initialize
    %% Inputs
    % Turbine Stirng
    Parameters.Turbine.String = 'SUMR-50_v6_s9';
    
    Linear.Name_FAedit         = 'A3_OFedit_Lin';  % Insert the name of the Fast/Aerodyn Edit Script
    Linear.Name_OpPt           = 'OpPts_SUMR-50_v6_s9_All';    % Operating point .csv file (wind speed, gen. speed, gen. torque, bld. pitch)
    Linear.Name_LinSaveDir     = Parameters.Turbine.String;       % Save Directory
    Linear.Name_LinSave        = 'SUMR-50_v6_s9';    
    scaled = 0;
    % will append with _<wind speed>
    
    % Load Turbine Params.
    ptemp = load(fullfile('ControlFiles\TurbineParameters\',Parameters.Turbine.String));
    Parameters.Turbine = catstruct(ptemp.Parameters.Turbine,Parameters.Turbine);
    
    % Linearization Params.
    Linear.B_opt= Parameters.Turbine.fine_pitch; % Blade pitch Threshold for Region 3        
    FAST.DT   = 0.0125;
    
    %% Set DOFs
    DOF.FlapDOF1    = 0;
    DOF.FlapDOF2    = 0;
    DOF.EdgeDOF     = 0;
    DOF.TeetDOF     = 0;
    DOF.DrTrDOF     = 0;
    DOF.GenDOF      = 1;
    DOF.YawDOF      = 0;
    DOF.TwFADOF1    = 0;
    DOF.TwFADOF2    = 0;
    DOF.TwSSDOF1    = 0;
    DOF.TwSSDOF2    = 0;
    
    
%% Set Up Linearizations
itic = tic;
%Turn 1s and 0s in to True and False
DOF.FieldStrings = fieldnames(DOF);
for iField = 1:length(DOF.FieldStrings)
    if eval(['DOF.',DOF.FieldStrings{iField}])
        eval(['DOF.',DOF.FieldStrings{iField}, '= ''True'';']);
    else
        eval(['DOF.',DOF.FieldStrings{iField}, '= ''False'';']);
    end
end
DOF = rmfield(DOF,'FieldStrings');

% disp(DOF)
%% Save Information
    PP.Save.Dir = ['../../Analysis/',Parameters.Turbine.String,'/Linearization/OpenFAST/Gen_DOF/'];
    PP.Save.Name = [Parameters.Turbine.String,'_','Linearization_'];
    PP.Save.Enable = 1;   
    
%% Turn LinTimes into string
% Set up LinTimes.  This is not currently being edited in real time due to
% some bugs in the Af_editFast Script need to fix
    Linear.NumAzSteps       = 20;
    Linear.StartTime        = 100;
    Linear.RotorPeriod      = 60/Parameters.Turbine.wr_rated;
    Linear.LinTimes         = Linear.StartTime:Linear.RotorPeriod/Linear.NumAzSteps:Linear.StartTime+Linear.RotorPeriod-Linear.RotorPeriod/Linear.NumAzSteps;
    Linear.TMax             = max(Linear.LinTimes)+5;
    FA.TMax                 = Linear.TMax;
Linear.TimesString = '';
for iLinTime = 1:length(Linear.LinTimes)
    Linear.TimesString = [Linear.TimesString,num2str(Linear.LinTimes(iLinTime))];
    if iLinTime < length(Linear.LinTimes)
        Linear.TimesString = [Linear.TimesString,','];
    end
end

%% Read in start points for trimming
A_OpPt      = csvread(fullfile('D:\SUMR_Data\',Linear.Name_LinSave ,'\Linearization\OpPts',[Linear.Name_OpPt,'.csv']));

% Trim for above rated (right now)
indOpPt     = A_OpPt(:,4)>=Linear.B_opt;
A_Wspd      = A_OpPt(indOpPt,1)';
A_RotSpd    = A_OpPt(indOpPt,2)/Parameters.Turbine.G;
A_GenTq     = A_OpPt(indOpPt,3);
A_Bp        = A_OpPt(indOpPt,4);
A_Cone      = A_OpPt(indOpPt,5);

IC_BpVec    = A_Bp;
IC_WrVec    = A_RotSpd;
IC_ConeVec  = A_Cone;

Shear       = 0; %  Set shear for const wind file

NumLins     = sum(indOpPt);

%%
S_RunMode=4;
A0_MAIN;
toc(itic);