namespace com.mitsukaki.poseengine.editor.generators
{
    public interface IPoseGenerator
    {
        public void Setup(PoseBuildContext context);

        public void BuildLayers(PoseBuildContext context);
        public void BuildStates(PoseBuildContext context);
    
        public void CleanUp(PoseBuildContext context);
    }
}