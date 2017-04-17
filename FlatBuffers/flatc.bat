flatc -n SyncObject.fbs SyncObjectID.fbs --gen-onefile
echo F | xcopy /Y /R *.cs ..\Assets\Scripts\FlatBuffers\
echo F | xcopy /Y /R *.fbs ..\Assets\Scripts\FlatBuffers\
@pause