
export const ItemType = {
	Book: 1,
	Music: 10,
	Clothing: 51
} as const;
export type ItemType = typeof ItemType[keyof typeof ItemType];
export interface Item {
	Type: ItemType;
	Id: number;
	Name: string;
}

