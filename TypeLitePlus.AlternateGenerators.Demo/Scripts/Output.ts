
export enum ItemType {
	Book = 1,
	Music = 10,
	Clothing = 51
}
export interface Item {
	Type: ItemType;
	Id: number;
	Name: string;
}

