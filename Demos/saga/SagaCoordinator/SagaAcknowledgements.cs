namespace SagaCoorindator;

public class SagaAcknowledgements
{
	public bool OrderAcknowledged { get; set; }
	public bool PaymentAcknowledged { get; set; }
	public bool InventoryAcknowledged { get; set; }
	public bool SagaIsComplete => OrderAcknowledged && PaymentAcknowledged && InventoryAcknowledged;
}