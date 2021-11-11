using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySharingManager : MonoBehaviour
{
    [SerializeField] private Button enter_room_button_, send_party_button_;
    [SerializeField] private Text room_name_text;
    [SerializeField] private InputField room_input_field_;

    private NetworkedClient client_;

    void Start()
    {
        enter_room_button_.onClick.AddListener(JoinSharingRoomButtonPressed);
        send_party_button_.onClick.AddListener(SendPartyButtonPressed);

        client_ = FindObjectOfType<NetworkedClient>();
    }

    void Update()
    {
        
    }

    private void JoinSharingRoomButtonPressed()
    {
        string name = room_input_field_.text;
        client_.SendMessageToHost(ClientToServerSignifiers.kJoinSharingRoom + ","+ name);
    }

    private void SendPartyButtonPressed()
    {
        AssignmentPart2.SendOnScreenPartyToServerForSharing(client_);
    }
}
