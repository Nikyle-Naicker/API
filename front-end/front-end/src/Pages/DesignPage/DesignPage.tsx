


type Props = {};

const tableConfig = [
    {
      label: "Market Cap",
      render: (company: any) => company.marketCapTTM,
      subTitle: "Total value of all a company's shares of stock",
    }
]

const DesignPage = (props: Props) => {
  return (
    <>
    <h1>Finshark Design Page</h1>
    <h2>This is Finshark's design page. This is where we house various design aspects of the app</h2>
    </>
  )
}

export default DesignPage