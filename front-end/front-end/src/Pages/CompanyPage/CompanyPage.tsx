import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { CompanyProfile } from '../../company';
import { getCompanyProfile } from '../api';
import Sidebar from '../../Components/Sidebar/Sidebar';
import CompanyDashboard from '../../Components/CompanyDashboard/CompanyDashboard';
import Tile from '../../Components/Tile/Tile';

interface Props {
    
}

const CompanyPage = (props: Props) => {
  //https:localhost:3000/
  let { ticker } = useParams();
  //console.log(ticker);
  const [company, setCompany] = useState<CompanyProfile>();

  useEffect(() => {
    const getProfileInit = async () => {
      const result = await getCompanyProfile(ticker!);
      setCompany(result?.data[0]);
    };
    getProfileInit();
  }, []);
  
  return (
    <>
      {company ? (
        <div className="w-full relative flex ct-docs-disable-sidebar-content overflow-x-hidden">

        <Sidebar/>
        <CompanyDashboard ticker={ticker!} >
          <Tile title="Company Name" subTitle={company.companyName}></Tile>
          <Tile title="Price" subTitle={company.price.toString()}></Tile>
          <Tile title="Currency" subTitle={company.currency}></Tile>
          <Tile title="Sector" subTitle={company.sector}></Tile>
          <p className='bg-white shadow rounded text-medium text-gray-900 p-3 mt-1 m-4' >
            {company.description}
          </p>
        </CompanyDashboard>

      </div>
      ) : (
        <div>Company Not Found!</div>
      )}
    </>

  );
};

export default CompanyPage