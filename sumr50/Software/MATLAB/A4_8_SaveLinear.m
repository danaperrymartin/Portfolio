%% A4_8_SaveLinear
% Inputs: PP.SaveDir
%         PP.SaveName

if ~isdir(PP.Save.Dir)
    mkdir(PP.Save.Dir);
end


% if strcmp(DOF.YawDOF,'True')
%     LinearCase = 1;
% elseif strcmp(DOF.GenDOF,'True') && strcmp(DOF.DrTrDOF,'True') && strcmp(DOF.FlapDOF1,'True') && strcmp(DOF.TwFADOF1,'True')
%     LinearCase = 9;
% elseif strcmp(DOF.GenDOF,'True') && strcmp(DOF.DrTrDOF,'True') && strcmp(DOF.TeetDOF,'True')
%     LinearCase = 11;
% elseif strcmp(DOF.GenDOF,'True') && strcmp(DOF.DrTrDOF,'True') && strcmp(DOF.FlapDOF1,'True')
%     LinearCase = 7;
% elseif strcmp(DOF.GenDOF,'True') && strcmp(DOF.DrTrDOF,'True')
%     LinearCase = 3;
% elseif strcmp(DOF.GenDOF,'True') && strcmp(DOF.TwFADOF1,'True') && strcmp(DOF.TeetDOF,'True') && strcmp(DOF.TwSSDOF1,'True')
%     LinearCase = 4;
% elseif strcmp(DOF.GenDOF,'True')
%     LinearCase = 2;
% else
    LinearCase = 0;
% end
%% Trim Matrices
% Inputs
indHorWindSpd   = find(strcmp(DescCntrlInpt,'IfW Extended input: horizontal wind speed (steady/uniform wind), m/s'));
indGenTq        = find(strcmp(DescCntrlInpt,'ED Generator torque, Nm'));
indBldPitchC    = find(strcmp(DescCntrlInpt,'ED Extended input: collective blade-pitch command, rad'));

indInputs       = [indHorWindSpd,indGenTq,indBldPitchC];

% Outputs
indRotSpeed     = find(strcmp(DescOutput,'ED RotSpeed, (rpm)'));
indGenSpeed     = find(strcmp(DescOutput,'ED GenSpeed, (rpm)'));
indRootMxc1     = find(strcmp(DescOutput,'ED RootMyc1, (kN·m)'));
indRootMxc2     = find(strcmp(DescOutput,'ED RootMyc2, (kN·m)'));
indTwrBsMyt     = find(strcmp(DescOutput,'ED TwrBsMyt, (kN·m)'));
indTwrBsMxt     = find(strcmp(DescOutput,'ED TwrBsMxt, (kN·m)'));
indLSShftMxa    = find(strcmp(DescOutput,'ED LSShftMxa, (kN·m)'));

indOutputs      = [indRotSpeed,indGenSpeed,indRootMxc1,indRootMxc2,indTwrBsMyt,indTwrBsMxt,indLSShftMxa];

if LinearCase       %has states
    % States
    indStates       = 2:2*NActvDOF;
    
    % Set avg. operating points to their value (exclude az as state)
    x_op     = mean(xop(indStates'),2);
    x_dop    = mean(xdop(indStates'),2);
    y_op     = mean(yop(indOutputs'),2);
    u_op     = mean(uop(indInputs'),2);
    
    
    % Trim Matrices
    A   = AvgAMat(indStates,indStates);
    B   = AvgBMat(indStates,indInputs);
    C   = AvgCMat(indOutputs,indStates);
    D   = AvgDMat(indOutputs,indInputs);
    
    % Make System Matrices
    P  = ss(A,B,C,D);
    
    % Set Names, Units
    P.InputName = {'HorWindSpd','GenTq','BldPitchC'};
    P.InputUnit = {'m/s','Nm','rad'};
    
    P.OutputName = {'RotSpeed','GenSpeed','RootMxc1','RootMxc2','TwrBsMyt','TwrBsMxt','LSShftMxa'};
    P.OutputUnit = {'rpm','rpm','kNm','kNm','kNm','kNm','kNm'};
    
end

%% Yaw DOF Enabled (for BP Gain Scheduling)

if LinearCase == 0
    %Get Aerodynamic Coefficients
    % do nothing
    
    indRotPwr       = find(strcmp(DescOutput,'ED RotPwr, (kW)'));
    indRotThrust    = find(strcmp(DescOutput,'ED RotThrust, (kN)'));
    
    indOutputs = [indRotPwr,indRotThrust];
    
    AeroGain = AvgDMat(indOutputs,indInputs);
    
end

%% Gen DOF Enabled

if LinearCase == 2
    
    P.StateName = {'dGeAz',};
    P.StateUnit = {'rad/s'};
    
end

%% Gen & DT DOF Enabled

if LinearCase == 3
    
    P.StateName = {'DrTr','dGeAz','dDrTr'};
    P.StateUnit = {'rad','rad','rad'};
    
end


%% Gen & DT, and Blade Flap DOF Enabled

if LinearCase == 7
    
    if NActvDOF == 5  % 3-bladed
        
        A = MBC_AvgA(indStates,indStates);
        B = mean(MBC_B(indStates,indInputs),3);
        C = mean(MBC_C(indOutputs,indStates),3);
        
        P = ss(A,B,C,D);
        
        P.StateName = {'DrTr','BF0','BFC','BFS','dTFA1','dGeAz','dDrTr','dBFC','dBFS'};
        P.StateUnit = {'rad', 'm',  'm',  'm', 'rad/s','rad/s','m/s','m/s','m/s'};

    else
        
        % transform to collective and differential mode
        T12 = [1/2,1/2;1/2,-1/2];
        
        T = [1,         zeros(1,2*NActvDOF-2);
            zeros(2,1),T12,zeros(2,2*NActvDOF-4);
            zeros(1,3),1,zeros(1,2*NActvDOF-5);
            zeros(1,4),1,zeros(1,2*NActvDOF-6);
            zeros(2,5),T12];
        
        for i = 1:size(AMat,3)
            ABar(:,:,i) = T*AMat(indStates,indStates,i)*inv(T);
            BBar(:,:,i) = T*BMat(indStates,indInputs,i);
            CBar(:,:,i) = CMat(indOutputs,indStates,i)*inv(T);
        end
        
        %DONT Remove teeter mode (for now)
        tempInd = 1:size(ABar,1); %[1,2,4,5,6];
        ABar_ = mean(ABar(tempInd,tempInd,:),3);
        BBar_ = mean(BBar(tempInd,:,:),3);
        CBar_ = mean(CBar(:,tempInd,:),3);
        
        
        P = ss(ABar_,BBar_,CBar_,D);
        
        P.StateName = {'DrTr','BF0','BF1','dGeAz','dDrTr','dBF0','dBF1'};
        P.StateUnit = {'rad', 'm', 'm', 'rad/s','rad/s','m/s','m/s'};
        
        % Re-Set Names, Units
        P.InputName = {'HorWindSpd','GenTq','BldPitchC'};
        P.InputUnit = {'m/s','Nm','rad'};
        
        P.OutputName = {'RotSpeed','GenSpeed','RootMxc1','RootMxc2','TwrBsMyt','TwrBsMxt','LSShftMxa'};
        P.OutputUnit = {'rpm','rpm','kNm','kNm','kNm','kNm','kNm'};
    end
    
end

%% Flexible Tower
if LinearCase == 9
    
    if NActvDOF == 6  % 3-bladed
        % do MBC stuff?
        indStates = [1,3:12];
        A = MBC_AvgA(indStates,indStates);
        B = mean(MBC_B(indStates,indInputs),3);
        C = mean(MBC_C(indOutputs,indStates),3);
        
        P = ss(A,B,C,D);
        
        
        P.StateName = {'TFA1','DrTr','BF0','BFC','BFS','dTFA1','dGeAz','dDrTr','dBF0','dBFC','dBFS'};
        P.StateUnit = {'m','rad', 'm',  'm',  'm', 'm/s', 'rad/s','rad/s','m/s','m/s','m/s'};
    else
        % Tower motion is first state, so change indStates
        indStates = [1,3:10];
        
        % transform to collective and differential mode
        T12 = [1/2,1/2;1/2,-1/2];
        
        T = [eye(2),         zeros(2,2*NActvDOF-3);
            zeros(2,2),T12,zeros(2,2*NActvDOF-5);
            zeros(3,4), eye(3), zeros(3,2*NActvDOF-8);
            zeros(2,7),T12];
        
        for i = 1:size(AMat,3)
            ABar(:,:,i) = T*AMat(indStates,indStates,i)*inv(T);
            BBar(:,:,i) = T*BMat(indStates,indInputs,i);
            CBar(:,:,i) = CMat(indOutputs,indStates,i)*inv(T);
            
            if 1   %debug
                modes(:,i) = eig(ABar(:,:,i));
            end
            
                
        end
        
        A = mean(ABar,3);
        B = mean(BBar,3);
        C = mean(CBar,3);
        
        P = ss(A,B,C,D);
        P.StateName = {'TFA1','DrTr','BF0','BF1','dTFA1','dGeAz','dDrTr','dBF0','dBF0'};
        P.StateUnit = {'m','rad', 'm',  'm', 'm/s','rad/s','rad/s','m/s', 'm/s'};
        
                % Re-Set Names, Units
        P.InputName = {'HorWindSpd','GenTq','BldPitchC'};
        P.InputUnit = {'m/s','Nm','rad'};
        
        P.OutputName = {'RotSpeed','GenSpeed','RootMxc1','RootMxc2','TwrBsMyt','TwrBsMxt','LSShftMxa'};
        P.OutputUnit = {'rpm','rpm','kNm','kNm','kNm','kNm','kNm'};
    end
    
end

%% Gen & Tower DOF Enabled

if LinearCase == 4
    
    % Set Names, Units
    P.StateName = {'TwrFA1','TwrSS1','GeAz','Teet','dTwrFA1','dTwrSS1','dGeAz','dTeet'};
    P.StateUnit = {'m','m','rad','rad','m/s','m/s','rad/s','rad/s'}';
    
end


%% Save

if LinearCase
   
    save(fullfile(PP.Save.Dir,[Linear.Name_LinSave,'_',num2str(A_Wspd(iOpPt)),'_',num2str(IC_Cone),'.mat']));
    
%     save(fullfile(SaveDir,[Linear.Name_LinSave,num2str(A_Wspd(iOpPt)),'\.','d')]),...
%         'xop','xdop','uop','yop','P','Parameters');
else
    
    save(fullfile(PP.Save.Dir,[Linear.Name_LinSave,'_',num2str(A_Wspd(iOpPt)),'_',num2str(IC_Cone),'.mat']));
    
%     save(fullfile(SaveDir,[Linear.Name_LinSave,regexprep(num2str(A_Wspd(iOpPt)),'\.','d')]),...
%         'AeroGain','uop','yop','Parameters');
end
