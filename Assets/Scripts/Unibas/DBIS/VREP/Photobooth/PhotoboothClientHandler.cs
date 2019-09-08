using Unibas.DBIS.VREP.Photobooth.Models;

namespace Unibas.DBIS.VREP.Photobooth
{
    public interface PhotoboothClientHandler
    {

        void HandleGetPostcards(PostcardsList list);
        void HandleRandomPostcard(PostcardsList list);
        void HandlePostSnapshot(IdObject idObject);
        void HandleGetHistory(HistoryList list);
        void HandleGetPrint(SuccessResponse response);
        void HandleError(string msg);
        void HandlePostcardInfo(ImageInfo obj);
        void HandleSelectedPostcard(PostcardsList obj);
    }
}