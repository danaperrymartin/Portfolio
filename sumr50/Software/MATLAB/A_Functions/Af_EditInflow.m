function Af_EditInflow(lines, edits, newname, defname)

%% Open Files
fidDefault  = fopen([defname,'.ipt'],'r');
if fidDefault==-1
    error(['Error: ', defname, '.ipt not found.  Note: you do not need to end string with .ipt']);
end
fidNew      = fopen([newname,'.ipt'],'w+');
if fidNew==-1
    error(['Error: ', newname, '.ipt not found.  Note: you do not need to end string with .ipt']);
end

%% If Edits are empty, copy file over and close
if isempty(edits)
    % If there are no edits, copy Master file
    copyfile([defname,'.ipt'],[newname,'.ipt']);
else
    % Copy Master file lines over and modify values
    while 1
        tline = fgetl(fidDefault);
        
        if ~ischar(tline)
            %Break @ end of file
            break;
        end
        
        editedLine = false;
        % Edit lines with user-defined values from MATLAB script
        for iLines = 1:length(lines)
            if strcmp(lines{iLines}(end-1:end),'_1')
                lines{iLines} = [lines{iLines}(1:end-2),'(1)'];
            elseif strcmp(lines{iLines}(end-1:end),'_2')
                lines{iLines} = [lines{iLines}(1:end-2),'(2)'];
            elseif strcmp(lines{iLines}(end-1:end),'_3')
                lines{iLines} = [lines{iLines}(1:end-2),'(3)'];
            end
            if ischar(tline)
                ntline = strfind(tline,'- ');
                ntline2 = strcmp(tline(1:3),'===');
            end
            if ~isempty(ntline)
                lineInd = strfind(tline(1:ntline),lines{iLines});
            elseif ~isempty(ntline2)
                lineInd = 0;
            else
                lineInd = strfind(tline,lines{iLines});
            end
            
            if lineInd
                editedLine = true;
                tlineMod = tline(lineInd:end);
                if isnumeric(edits{iLines})
                    edits{iLines} = pad(num2str(edits{iLines}),lineInd-4,'left');
                    edits{iLines} = pad(edits{iLines},length(edits{iLines})+3,'right');
                elseif strcmp('"default"',edits{iLines})
                    edits{iLines} = pad(edits{iLines},lineInd-4,'left');
                    edits{iLines} = pad(edits{iLines},length(edits{iLines})+3,'right');
                else
                    edits{iLines} = pad(edits{iLines},lineInd-4,'right');
                    edits{iLines} = pad(edits{iLines},length(edits{iLines})+3,'right');
                end
                fprintf(fidNew,'%s%s\n',edits{iLines},tlineMod);            
            end            
        end
        
        if ~editedLine
            % If we don't need to edit, print original line
            fprintf(fidNew,'%s\n',tline);
        end
    end
end

%% Close Files

fclose('all');
disp(['Status: ', newname,'.ipt created with ',num2str(length(edits)),' changed input(s).']);

