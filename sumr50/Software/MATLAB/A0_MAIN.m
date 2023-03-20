%% A0_Main
% J.Aho 9/29/12

% This Main function is called by the A2_* Files
% to execute simulations, freq tests, linearizations, etc.
% Please see Readme for more details

% Under normal conditions you should not have to edit anything below
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


%% S_RunMode=1 (Simulation)
if S_RunMode==1
    
    %Now using StartFcn callback...A3_* needs this
    cd('ControlFiles');
    eval(Simulation.Name_Control);
    cd('..');
    % 2.2.3 Change/Load FAST Parameters
    eval(Simulation.Name_FAedit)
    FAST_InputFileName = fullfile(PP.Save.Dir,'FASTinput.fst');
    % Create wind file
    if IW.WindType==1||IW.WindType==3
    WindDir = PP.Save.Dir;
    elseif IW.WindType~=1 
     WindDir = PP.Save.Dir;
    [Wtype,W]=Af_MakeWind(WindDir,Disturbance.Type, WindParams1(1),WindParams1(2),WindParams1(3),WindParams1(4),Hshear,Vshear,LVshear,WindParams1(8),WindParams2,0);
    clear WindParams1 WindParams2  Vshear Hshear GS VS WD LVshear
    end
%     Read_FAST_Input
    
    % 2.2.4 Try Simulation
    disp('-------------------------------------------------------------------------------')
    fprintf('Running simulink model %s with %s\n', Simulation.Name_Model, Simulation.Name_Control);

%     try
    sim([Simulation.Name_Model])
     error=0;

    %% Post Processing
    if PP.Enabled
        for p=1:length(PP.Scripts)
            if ~isempty(PP.Scripts{p})
                eval(PP.Scripts{p})
            end
        end
    end
    
    
%     catch
%     error = 1;
%     display('Error Ocurred');
%     
%     if PP.Enabled
%         for p=1:length(PP.Scripts)
%             if ~isempty(PP.Scripts{p})
%                 eval(PP.Scripts{p})
%             end
%         end
%     end
%      end
end

%% S_RunMode=2 (Const)

if S_RunMode==2
    
    eval(Name_Control)
    A_OpPts=zeros(length(A_Wspd),4);
    
    for iOpPt=1:length(A_Wspd)
        
        disp(['Running Sim ',num2str(iOpPt),' of ',num2str(length(A_Wspd))])
        
        IC_Wr=IC_WrVec(iOpPt);
        IC_BP=IC_BPVec(iOpPt);
        
        eval(Name_FAedit)
        
        SimsetupEdit
        
        Af_MakeConstWind(A_Wspd(iOpPt), TMax, DT, Vshear)
        
        eval(Name_FAedit)
        
        sim([Name_Model,'.mdl'])
        disp('Simulation has completed running')
        toc
        
        figure(8);
        subplot(2,1,1);
        
        A_OpPts(iOpPt,:)=[A_Wspd(iOpPt),...
            mean(OutData(Tsi:end,strmatch('GenSpeed',OutList))),...
            mean(OutData(Tsi:end,strmatch('GenTq',OutList)))*1000,...
            mean(OutData(Tsi:end,strmatch('BldPitch1',OutList)))];
    end
    
    if S_Plots
        figure, Af_5MWSpdTqPlot, hold on, scatter(A_OpPts(:,2),A_OpPts(:,3),'rX')
    end
    csvwrite(['SaveData\OpPts\OpPts_',Name_OpPt,'.csv'],A_OpPts)
    
    A_OpPts
end


%% S_RunMode=3 (Frequency Test)
if S_RunMode==3
    
    %eval(Name_Control)
    
    Total=length(A_Wspd)*length(A_Freq)*length(A_Amp);
    Run=0;
    for iOpPt=1:length(A_Wspd)
        for A_IF=1:length(A_Freq)
            for A_IA=1:length(A_Amp)
                Run=Run+1;
                
                disp(['Running Sim ',num2str(Run),' of ',num2str(Total)])
                
                F_InData=A_Amp(A_IA)*sin(2*pi*A_Freq(A_IF)*A_Time); %Sets input data stream
                
                if FtestInput==1 % Wind Input
                    WindParams2(1:3)=[A_Wspd(iOpPt),A_Freq(A_IF),A_Amp(A_IA)];    % (DC, freq, amp)
                    In_Tq(:,2)=ones(L,1)*A_GenTq(iOpPt);
                    In_Bp(:,2)=ones(L,1)*A_Bp(iOpPt);
                    WtypeID=4;
                else
                    WindParams2(1)=A_Wspd(iOpPt);
                    WtypeID=1;
                    if FtestInput==2 % BP input
                        In_Tq(:,2)=ones(L,1)*A_GenTq(iOpPt);
                        In_Bp(:,2)=A_Bp(iOpPt)+F_InData;
                    elseif FtestInput==3 %Tq Input
                        In_Tq(:,2)=A_GenTq(iOpPt)+F_InData;
                        In_Bp(:,2)=ones(L,1)*A_Bp(iOpPt);
                    end
                end
                
                IC_Wr=IC_WrVec(iOpPt);
                IC_Bp=IC_BpVec(iOpPt);
                eval(Name_FAedit)
                
                Wtype=Af_MakeWind(WtypeID, WindParams1(1),WindParams1(2),WindParams1(3),WindParams1(4),Hshear,Vshear,LVshear,WindParams1(8),WindParams2,0);
                
                SimsetupEdit
                sim([Name_Model,'.mdl'])
                disp('Simulation has completed running')
                toc
                
                if FtestOutput==1
                    F_OutData(:,:)=[qStates(Tsi:end,:),qdotStates(Tsi:end,:)];
                elseif  FtestOutput==2
                    for n=1:length(FtestOutDesc)
                        F_OutData(:,n)=OutData(Tsi:end,strmatch(FtestOutDesc{n},OutList));
                    end
                end
                if Run==1, Af_StateAso, end
                
                A4_3_Freq_MakeBode
                
            end
        end
    end
    
    
    if S_FreqPlots==1
        A4_3_Freq_Plots
    end
    
end


%% S_RunMode=4 (Linearization)
if S_RunMode==4
    mkdir(PP.Save.Dir)
    
    for iOpPt=1:NumLins
        
        IC_Bp=IC_BpVec(iOpPt);
        IC_Wr=IC_WrVec(iOpPt);
        IC_Cone = IC_ConeVec(iOpPt);
        
        eval(Linear.Name_FAedit)
        
%         Af_MakeConstWind(A_Wspd(A_IW), TMax, DT, 0)
        
        disp(['Running Sim ',num2str(iOpPt),' of ',num2str(length(A_Wspd))])
        filename = [Name_FAST_New '.fst'];
%         cd('..\');
        system(['..\build\bin\FAST.exe ' filename ]);
%         cd('..');
        
        %% Get State Space Matrices
        FileNames = cell(1,length(Linear.LinTimes));
        for iLinTime = 1:length(Linear.LinTimes)
            FileNames{iLinTime} = [PP.Save.Dir,'FASTinput.',num2str(iLinTime),'.lin'];
        end
        GetMats_f8;
        mbc3;
        
        A4_8_SaveLinear
        
%         save([SaveDir,'\',Name_LinSave,num2str(A_Wspd(A_IW)),'.mat'],'Pd','Pc','y_op','u_op','x_op','Desc*');
%         if S_SaveCamp
%             A4_4_Campbell
%         end
        
    end
    
end


%% S_Run_Mode == 5 (Linear SS Simulations)
if S_RunMode==5
    
    
    
    for sim_iter=1:length(A_Wspd)
        
        %load up linearization params & Wind
        load(fullfile(SaveDir,[Name_LinSave,num2str(sim_iter)]),...
            'Avg*','Desc*','*op*');
        load(fullfile(SaveDir,[Wtype,num2str(sim_iter)]));
        
        eval(Name_Control)
        
        disp(['Running Sim ',num2str(sim_iter),' of ',num2str(length(A_Wspd)),...
            ', Pitch = ',num2str(Params.Lin.u_op*180/pi),' deg  Wind Speed = ',...
            num2str(Params.Lin.v_op)]);
        
        sim([Name_Model,'.mdl'])
        disp('Simulation has completed running')
        
        save(fullfile(SaveDir,['OutData',num2str(sim_iter)]),...
            'OutData','OutList','Params','Dist');
        
        if S_PP,
            for p=1:length(Name_PP), eval(Name_PP{p}),  end
        end
    end
    
end
%% Wind Farm Simulation
if S_RunMode==6 
    
    %Now using StartFcn callback...A3_* needs this
    cd('ControlFiles');
    eval(Simulation.Name_Control);
    cd('..');
    % 2.2.3 Change/Load FAST Parameters
    eval(Simulation.Name_FAedit)
    FAST_InputFileName = './FAST8_IF/FASTinput.fst';
%     Read_FAST_Input
    
    % 2.2.4 Try Simulation
    disp('-------------------------------------------------------------------------------')
    fprintf('Running simulink model %s with %s\n', Simulation.Name_Model, Simulation.Name_Control);

    sim([Simulation.Name_Model])
     error=0;
end



%% Notify User of Completion
% toc
disp(['A0_MAIN has finished running in mode ', num2str(S_RunMode)])
disp(['Thank you for playing!'])


% i=1;
% while i>.2
%     beep
%     pause(i)
%     i=i*.7;
% end
