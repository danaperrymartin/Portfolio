%% Af_getFileTable
% Loads input file templates from Google Sheet "Turbine Model Summary"
%% Load Sheet
GoogleSheetKey = '1krW5x0sCx_bACuxsqp2Pfu4_4JNjFpkHnXd7wyOu_I8';    
gid_TradeStudy = {'2062739333'};    %SUMR-D
% gid_TradeStudy = {'77484489'};      %NREL-5MW
% gid_TradeStudy = {'28997196'};      %CONR
% gid_TradeStudy = {'749174259'};     %SUMR-13i
% gid_TradeStudy = {'1788854053'};    %SUMR-13
% gid_TradeStudy = {'1801338246'};    %SUMR-25
% gid_TradeStudy = {'558326597'};       %SUMR-50

ModelString = {};
ModelNumber = 0;
for iSheet = 1:length(gid_TradeStudy)
    
    result = GetGoogleSpreadsheet(GoogleSheetKey,gid_TradeStudy{iSheet});
    SheetModels = result(1,2:end);
    
    ModelString = [ModelString , SheetModels];
    FileString = result(:,1);
    
    for iModel = 1:length(SheetModels)
        FAST_Template{iModel+ModelNumber}   = result{strcmp('Fast Input',FileString),strcmp(SheetModels(iModel),result(1,:))};
        ED_Template{iModel+ModelNumber}     = result{strcmp('ElastoDyn Input',FileString),strcmp(SheetModels(iModel),result(1,:))};
        ED_BladeFile{iModel+ModelNumber}    = result{strcmp('Blade Input (ED)',FileString),strcmp(SheetModels(iModel),result(1,:))};
        AD14_Template{iModel+ModelNumber}   = result{strcmp('AeroDyn Input (v14)',FileString),strcmp(SheetModels(iModel),result(1,:))};
        AD15_Template{iModel+ModelNumber}   = result{strcmp('AeroDyn Input (Open)',FileString),strcmp(SheetModels(iModel),result(1,:))};
        AD15_BladeFile{iModel+ModelNumber}  = result{strcmp('Blade Geometry',FileString),strcmp(SheetModels(iModel),result(1,:))};
        SD_Template{iModel+ModelNumber}     = result{strcmp('ServoDyn Input',FileString),strcmp(SheetModels(iModel),result(1,:))};
        IW_Template{iModel+ModelNumber}     = result{strcmp('InflowWind Input',FileString),strcmp(SheetModels(iModel),result(1,:))};
        BD_BladeFile{iModel+ModelNumber}    = result{strcmp('BeamDyn Blade',FileString),strcmp(SheetModels(iModel),result(1,:))};
        BD_Template{iModel+ModelNumber}     = result{strcmp('BeamDyn Input',FileString),strcmp(SheetModels(iModel),result(1,:))};
        Tower_Template{iModel+ModelNumber}  = result{strcmp('Tower Input',FileString),strcmp(SheetModels(iModel),result(1,:))};
        
    end
    
    ModelNumber = ModelNumber + length(SheetModels);
    
    
    
end


save(fullfile('C:\sumr50-repo\Software\MATLAB\','FAST8_IF','TemplateTable'),'ModelString','FAST_Template',...
    'ED_Template','ED_BladeFile','AD14_Template','AD15_Template','AD15_BladeFile',...
    'SD_Template','IW_Template','BD_BladeFile','BD_Template','Tower_Template');

